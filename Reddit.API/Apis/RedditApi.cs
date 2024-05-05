
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text;

namespace Reddit.API;

public static class RedditApi
{

    public static RouteGroupBuilder MapRedditApiV1(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("api/reddit");
        api.MapPost("/", UpdateReddit);
        return api;
    }


    public static async Task<Results<Ok<RedditApiResponse>, BadRequest<string>>> UpdateReddit([AsParameters] RedditService service, [FromQuery] string reddit, [FromHeader(Name = "statistics")] string statistics, [FromHeader(Name = "x-requestid")] Guid requestId)
    {

        service.Logger.LogInformation("Request to log list to cache");
        var updateRedditCommand = new UpdateRedditCommand(requestId, reddit, statistics.Split());
        
        var commandResult = await service.Mediator.Send(updateRedditCommand);
        var response = new RedditApiResponse()
        {
            ListName = commandResult.RedditListName,
            HitsLeft = commandResult.HitsLeft,
            HitsUsed = commandResult.HitsUsed,
        };

        service.Logger.LogInformation($"Successfully logged and loaded {commandResult} to/from cache. YYY was not able to be logged to cache");
        
        return TypedResults.Ok(response);

    }

}

public record RedditApiResponse
{
    public string ListName { get; set; }
    public int HitsUsed { get; set; }
    public int HitsLeft { get; set; }
}