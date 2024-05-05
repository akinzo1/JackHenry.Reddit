using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace RedditProcessor.Services;

public class RedditManagerService(HttpClient httpClient, ILogger<RedditManagerService> logger) : BackgroundService
{

    private readonly string remoteServiceBaseUrl = "/api/reddit";
    private readonly string[] redditList = { "bitcoin", "technology", "MadeMeSmile", "dotnet" };
    private readonly string[] statistics = { "MostUpVotes", "MostPosts" };
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("This logger working at: {time}", DateTimeOffset.Now);
            }

            foreach (var reddit in redditList)
            {

                //Make api call to handle the updating of the cache
                var apiUrl = QueryHelpers.AddQueryString(remoteServiceBaseUrl, "reddit", reddit);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                requestMessage.Headers.Add("x-RequestId", Guid.NewGuid().ToString());
                requestMessage.Headers.Add("statistics", statistics);

                var result = await httpClient.SendAsync(requestMessage);
                //if success/failure, update the string builder to log to console at the end
                var resp = await result.Content.ReadFromJsonAsync<RedditApiResponse>();

                
                // Make the calculation for how long to delay for


                await Task.Delay(1000, stoppingToken);

            }

            logger.LogInformation($"Loaded {redditList} at {DateTime.Now}");
        }
    }
}

public record RedditApiResponse
{
    public string ListName { get; set; }
    public int HitsUsed { get; set; }
    public int HitsLeft { get; set; }
}
