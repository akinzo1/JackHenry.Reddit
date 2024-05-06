using Reddit.API.Model;
using Reddit.API.Model.Api;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Reddit.API.Model.Api.SubRedditApiResponse;

namespace Reddit.API.Repository
{
    public class RedisRedditRepository(ILogger<RedisRedditRepository> logger, IConnectionMultiplexer redis) : IRedditRepository
    {
        private static RedisKey ListKeyPrefix = "/list/"u8.ToArray();
        private readonly IDatabase _database = redis.GetDatabase();
        private static string CacheName = "CacheLimits";

        private static RedisKey GetListKey(string id) => ListKeyPrefix.Append(id);

        public async Task<bool> DeleteLimitsAsync()
        {
            return await _database.KeyDeleteAsync(GetListKey(CacheName));

        }

        public async Task<bool> DeleteListAsync(string id)
        {
            return await _database.KeyDeleteAsync(GetListKey(id));
        }

        public async Task<ApiLimits?> GetLimitsAsync()
        {
            using var data = await _database.StringGetLeaseAsync(GetListKey(CacheName));
            
            if (data is null || data.Length == 0)
            {
                return null;
            }
            return JsonSerializer.Deserialize(data.Span, APILimitsSerializationContext.Default.ApiLimits);
        }

        public async Task<SubRedditApiResponse?> GetListAsync(string subreddit)
        {
            using var data = await _database.StringGetLeaseAsync(GetListKey(subreddit));

            if (data is null || data.Length == 0)
            {
                return null;
            }
            return JsonSerializer.Deserialize(data.Span, RedditSerializationContext.Default.SubRedditApiResponse);
        }

        public async Task<ApiLimits?> UpdateLimitsAsync(ApiLimits limits)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(limits, APILimitsSerializationContext.Default.ApiLimits);
            var created = await _database.StringSetAsync(GetListKey(CacheName), json);

            if (!created)
            {
                logger.LogInformation("Problem occurred persisting the item.");
                return null;
            }

            logger.LogInformation($"{limits} persisted successfully.");
            return await GetLimitsAsync();

        }

        public async Task<SubRedditApiResponse?> UpdateListAsync(SubRedditApiResponse reddit)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(reddit, RedditSerializationContext.Default.SubRedditApiResponse);
            var created = await _database.StringSetAsync(GetListKey(reddit.SubRedditName), json);

            if (!created)
            {
                logger.LogInformation("Problem occurred persisting the item.");
                return null;
            }

            logger.LogInformation($"{reddit} persisted successfully.");
            return await GetListAsync(reddit.SubRedditName);
        }
    }
}


[JsonSerializable(typeof(SubRedditApiResponse))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class RedditSerializationContext : JsonSerializerContext
{

}


[JsonSerializable(typeof(ApiLimits))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class APILimitsSerializationContext : JsonSerializerContext
{

}