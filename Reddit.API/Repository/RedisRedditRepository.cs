using Reddit.API.Model;
using Reddit.API.Model.Api;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.API.Repository
{
    public class RedisRedditRepository(ILogger<RedisRedditRepository> logger, IConnectionMultiplexer redis) : IRedditRepository
    {
        private static RedisKey ListKeyPrefix = "/list/"u8.ToArray();
        private readonly IDatabase _database = redis.GetDatabase();

        private static RedisKey GetListKey(string id) => ListKeyPrefix.Append(id);

        public async Task<bool> DeleteListAsync(string id)
        {
            return await _database.KeyDeleteAsync(GetListKey(id));
        }

        public async Task<SubRedditApiResponse?> GetListAsync(string subRedditName)
        {
            using var data = await _database.StringGetLeaseAsync(GetListKey(subRedditName));

            if (data is null || data.Length == 0)
            {
                return null;
            }
            return JsonSerializer.Deserialize(data.Span, RedditSerializationContext.Default.SubRedditApiResponse);
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

