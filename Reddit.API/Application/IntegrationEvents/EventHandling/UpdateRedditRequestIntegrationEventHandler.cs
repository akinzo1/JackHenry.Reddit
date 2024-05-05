using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Reddit.API.Application.IntegrationEvents.Events;
using Reddit.API.Model.Api;
using Reddit.API.Model.Configuration;
using System;
using System.Net.Http;

namespace Reddit.API.Application.IntegrationEvents.EventHandling;

public class UpdateRedditRequestIntegrationEventHandler(IOptions<RedditSettings> redditSettings, HttpClient httpClient, IRedditRepository _repository, ILogger<UpdateRedditRequestIntegrationEventHandler> logger) : IIntegrationEventHandler<UpdateRedditRequestIntegrationEvent>
{
    private static readonly string baseUrl = "https://oauth.reddit.com/";

    public async Task Handle(UpdateRedditRequestIntegrationEvent @event)
    {
        logger.LogInformation($"Handling integration event: {@event.Id} - ({@event})");

        //Make api call to handle the updating of the cache
        var apiUrl = QueryHelpers.AddQueryString($"{baseUrl}{@event.statistic}", "reddit", @event.reddit);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", redditSettings.Value.Token);

        var result = await httpClient.SendAsync(requestMessage);
        //if success/failure, update the string builder to log to console at the end
        var response = await result.Content.ReadFromJsonAsync<SubReddit>();




        //store the limits into cache

        var t = await _repository.GetListAsync(@event.reddit);


        await Task.Delay(100);

    }
}

