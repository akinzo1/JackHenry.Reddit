using Reddit.API.Model;
using Reddit.API.Model.Api;

namespace Reddit.API;

public interface IRedditRepository
{
    Task<SubRedditApiResponse?> GetListAsync(string reddit);
    Task<SubRedditApiResponse?> UpdateListAsync(SubRedditApiResponse listName);
    Task<bool> DeleteListAsync(string id);
}
