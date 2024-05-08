using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reddit.API.Application.IntegrationEvents.Events;
using Reddit.API.Model.Api;
using Reddit.API.Model.Configuration;
using System;
using System.Net.Http;
using static Reddit.API.Model.Api.SubRedditApiResponse;

namespace Reddit.API.Application.IntegrationEvents.EventHandling;

public class UpdateRedditRequestIntegrationEventHandler(IOptions<RedditSettings> redditSettings, HttpClient httpClient, IRedditRepository _repository, ILogger<UpdateRedditRequestIntegrationEventHandler> logger, IHubContext<NotificationsHub> _hubContext) : IIntegrationEventHandler<UpdateRedditRequestIntegrationEvent>
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

        if (result.IsSuccessStatusCode)
        {
            var response = await result.Content.ReadAsStringAsync();
            var redditResponse = JsonConvert.DeserializeObject<SubRedditApiResponse>(response);
            var rateLimits = new ApiLimits();

            if (redditResponse != null)
            {

                redditResponse.SubRedditName = @event.subReddit;
                redditResponse.RequestDateTime = DateTime.UtcNow;

                // this can be cleaner
                rateLimits.RateLimit_Reset = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-reset").Value.FirstOrDefault())));
                rateLimits.RateLimit_Remaining = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-remaining").Value.FirstOrDefault())));
                rateLimits.RateLimit_Used = Convert.ToInt32(Math.Floor(Convert.ToDouble(result.Headers.FirstOrDefault(o => o.Key == "x-ratelimit-used").Value.FirstOrDefault())));

                //store response and ratelimits into cache
                await _repository.UpdateListAsync(redditResponse);
                await _repository.UpdateLimitsAsync(rateLimits);

                // depending on the statistic, use factory to get the exact data needed then save that
                // Use signalr to send the upvotes/mostposts to UI 
                if (@event.statistic == "MostUpVotes")
                {
                    var upVotes = redditResponse.Data.Children.OrderByDescending(o => o.Data.Ups).Select(o => o.Data);
                    await _hubContext.Clients.All.SendAsync("RedditUpVotesUpdated", upVotes.ToList(), redditResponse.SubRedditName, redditResponse.RequestDateTime);
                }
                else
                {
                    var usersWithMostPosts = redditResponse.Data.Children.GroupBy(o => o.Data.Author).Select(i => new UserCounts() { Author = i.First().Data.Author, TotalPosts = i.Count() }).OrderByDescending(i => i.TotalPosts).ThenBy(o => o.Author);
                    await _hubContext.Clients.All.SendAsync("RedditMostPostsUpdated", usersWithMostPosts.ToList());
                }

            }

        }
        else
        {

            //update the reddit for that cache back to "Needs to be reset"
            throw new HttpRequestException($"Could not load {apiUrl}. ResponseMessage: {result}. Event: {@event}");

        }

    }
}

