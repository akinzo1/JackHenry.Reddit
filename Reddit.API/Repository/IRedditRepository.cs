using Reddit.API.Model;

namespace Reddit.API;

public interface IRedditRepository
{
    Task<RedditList?> GetListAsync(string reddit);
    Task<RedditList?> UpdateListAsync(RedditList listName);
    Task<bool> DeleteListAsync(string id);
}
