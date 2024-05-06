using Reddit.API.Model;
using Reddit.API.Model.Api;

namespace Reddit.API;

public interface IRedditRepository
{
    Task<SubRedditApiResponse?> GetListAsync(string subreddit);
    Task<SubRedditApiResponse?> UpdateListAsync(SubRedditApiResponse subreddit);
    Task<bool> DeleteListAsync(string subreddit);
    Task<ApiLimits?> GetLimitsAsync();
    Task<ApiLimits?> UpdateLimitsAsync(ApiLimits limits);
    Task<bool> DeleteLimitsAsync();
}
