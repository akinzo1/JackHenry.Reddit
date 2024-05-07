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

        var delay = 1000;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("This logger working at: {time}", DateTimeOffset.Now);
            }

            foreach (var reddit in redditList)
            {

                //Make api call to handle the updating of the cache
                var apiUrl = QueryHelpers.AddQueryString(remoteServiceBaseUrl, "subreddit", reddit);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                requestMessage.Headers.Add("x-RequestId", Guid.NewGuid().ToString());
                requestMessage.Headers.Add("statistics", statistics);

                var result = await httpClient.SendAsync(requestMessage);
                
                var response = await result.Content.ReadFromJsonAsync<ApiLimits>();

                if (response != null)
                {
                    int remaining = response.RateLimit_Remaining;
                    int used = response.RateLimit_Used;
                    int reset = response.RateLimit_Reset;

                    delay = CalculateDelay(remaining, reset, used);

                    logger.LogInformation($"Requested to update {reddit}. Statistic: {statistics.ToString()}. Requests remaining {response.RateLimit_Remaining}. Request Used {response.RateLimit_Used}. Request Reset: {response.RateLimit_Reset}");

                    Console.WriteLine($"Delay time is {delay}ms");

                }

                await Task.Delay(delay, stoppingToken);

            }

            logger.LogInformation($"Loaded {redditList} at {DateTime.Now}");
        }
    }

    private int CalculateDelay(int remaining, int reset, int used)
    {
        var rate = Convert.ToDouble(remaining - 4 > 0 ? remaining - 4 : remaining) / Convert.ToDouble(reset + 5);

        return rate == 0 ? 1000 : (int)((1 / rate) * 1000);

    }
}

public class ApiLimits
{
    public int RateLimit_Remaining { get; set; }
    public int RateLimit_Used { get; set; }
    public int RateLimit_Reset { get; set; }
}