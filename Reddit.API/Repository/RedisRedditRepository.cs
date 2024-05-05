using Reddit.API.Model;
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

        public async Task<RedditList?> GetListAsync(string redditListName)
        {
            using var data = await _database.StringGetLeaseAsync(GetListKey(redditListName));

            if (data is null || data.Length == 0)
            {
                return null;
            }
            return JsonSerializer.Deserialize(data.Span, RedditSerializationContext.Default.RedditList);
        }

        public async Task<RedditList?> UpdateListAsync(RedditList reddit)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(reddit, RedditSerializationContext.Default.RedditList);
            var created = await _database.StringSetAsync(GetListKey(reddit.ListName), json);

            if (!created)
            {
                logger.LogInformation("Problem occurred persisting the item.");
                return null;
            }

            logger.LogInformation($"{reddit.ListName} persisted successfully.");
            return await GetListAsync(reddit.ListName);
        }
    }
}


[JsonSerializable(typeof(RedditList))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class RedditSerializationContext : JsonSerializerContext
{

}

