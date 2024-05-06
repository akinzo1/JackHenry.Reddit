using System.Net.Http.Json;
using System.Runtime.InteropServices;
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


            // Get RedditList that didn't get to be updated and add to the redditList. Process those first



            foreach (var reddit in redditList)
            {

                //Make api call to handle the updating of the cache
                var apiUrl = QueryHelpers.AddQueryString(remoteServiceBaseUrl, "subreddit", reddit);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                requestMessage.Headers.Add("x-RequestId", Guid.NewGuid().ToString());
                requestMessage.Headers.Add("statistics", statistics);

                var result = await httpClient.SendAsync(requestMessage);
                //if success/failure, update the string builder to log to console at the end
                var response = await result.Content.ReadFromJsonAsync<ApiLimits>();


                // Make the calculation for how long to delay for
                if (response != null)
                {
                    logger.LogInformation($"Requested to update {reddit}. Requests remaining {response.RateLimit_Remaining}. Request Used {response.RateLimit_Used}. Request Reset: {response.RateLimit_Reset}");
                }


                // With this new information, plan to delay proportionally
                await Task.Delay(1000, stoppingToken);

            }

            logger.LogInformation($"Loaded {redditList} at {DateTime.Now}");
        }
    }
}

public class ApiLimits
{
    public int RateLimit_Remaining { get; set; }
    public int RateLimit_Used { get; set; }
    public int RateLimit_Reset { get; set; }
}