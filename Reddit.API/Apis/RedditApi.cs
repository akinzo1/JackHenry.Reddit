
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Reddit.API.Model.Api;
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
        var updateRedditCommand = new UpdateRedditCommand(requestId, reddit, statistics.Split(","));
        var commandResult = await service.Mediator.Send(updateRedditCommand);

        var response = new RedditApiResponse()
        {
            RedditListName = commandResult.RedditListName,
            Used = commandResult.Used,
            Remaining = commandResult.Remaining,
            Reset = commandResult.Reset
        };

        service.Logger.LogInformation($"Successfully logged and loaded {commandResult.RedditListName} to/from cache. YYY was not able to be logged to cache");

        return TypedResults.Ok(response);

    }

}
