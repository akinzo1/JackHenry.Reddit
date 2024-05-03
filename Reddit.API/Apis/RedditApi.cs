
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using System.Text;

namespace Reddit.API;

public static class RedditApi
{

    public static RouteGroupBuilder MapRedditApiV1(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api/reddit").HasApiVersion(1.0);
        api.MapPost("/", UpdateReddit);
        return api;
    }


    public static async Task<string> UpdateReddit()
    {

        //var cardTypes = await orderQueries.GetCardTypesAsync();
        
        var command = new SetRedditCommand("list");

        return await GetString();

    }

    private static async Task<string> GetString()
    {
        return await Task.FromResult("This string");
    }

}

public record SetRedditCommand(
string list
);