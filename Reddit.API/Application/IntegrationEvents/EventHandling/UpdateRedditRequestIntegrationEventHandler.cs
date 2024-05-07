using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reddit.API.Application.IntegrationEvents.Events;
using Reddit.API.Model.Api;
using Reddit.API.Model.Configuration;
using System;
using System.Net.Http;

namespace Reddit.API.Application.IntegrationEvents.EventHandling;

public class UpdateRedditRequestIntegrationEventHandler(IOptions<RedditSettings> redditSettings, HttpClient httpClient, IRedditRepository _repository, ILogger<UpdateRedditRequestIntegrationEventHandler> logger) : IIntegrationEventHandler<UpdateRedditRequestIntegrationEvent>
{
    private static readonly string baseUrl = "https://oauth.reddit.com/search";

    public async Task Handle(UpdateRedditRequestIntegrationEvent @event)
    {
        logger.LogInformation($"Handling integration event: {@event.Id} - ({@event})");

        var queries = new List<KeyValuePair<string, string>>
        {
            new("q", @$"subreddit:{@event.subReddit}")
        };

        //Make api call to handle the updating of the cache
        var apiUrl = QueryHelpers.AddQueryString($"{baseUrl}", queries);

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", redditSettings.Value.Token);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ChangeMeClient/0.1");

        var result = await httpClient.GetAsync(apiUrl);

        if (result.IsSuccessStatusCode != null)
        {
            var response = await result.Content.ReadAsStringAsync();

            //var upVotes = response.Data.Children.OrderByDescending(o => o.Data.Ups).Select(o => o.Data);
            //if success/failure, update the string builder to log to console at the end

            var redditParsed = JsonConvert.DeserializeObject<SubRedditApiResponse>(response);
            var rateLimits = new ApiLimits();

            redditParsed.SubRedditName = @event.subReddit;
            redditParsed.RequestDateTime = DateTime.UtcNow;

            rateLimits.RateLimit_Reset = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-reset").Value.FirstOrDefault())));
            rateLimits.RateLimit_Remaining = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-remaining").Value.FirstOrDefault())));
            rateLimits.RateLimit_Used = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-used").Value.FirstOrDefault())));

            //store response and ratelimits into cache
            await _repository.UpdateListAsync(redditParsed);
            await _repository.UpdateLimitsAsync(rateLimits);


        }
        else
        {

            //update the reddit for that cache back to "Needs to be reset"
            throw new HttpRequestException($"Could not load {apiUrl} for {@event}");



        }

    }
}

