using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace RedditProcessor.Services;

public class RedditManagerService(HttpClient httpClient, ILogger<RedditManagerService> logger) : BackgroundService
{

    private readonly string remoteServiceBaseUrl = "/api/reddit";

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

            //Make api call to handle the updating of the cache
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
            requestMessage.Headers.Add("x-requestid", Guid.NewGuid().ToString());

            var result = await httpClient.SendAsync(requestMessage);
            var resp = await result.Content.ReadAsStringAsync();

            logger.LogInformation($"Loaded {resp} at {DateTime.Now}");

            await Task.Delay(10000, stoppingToken);
        }
    }
}
