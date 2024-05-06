
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


    public static async Task<Results<Ok<ApiLimits>, BadRequest<string>>> UpdateReddit([AsParameters] RedditService service, [FromQuery] string subreddit, [FromHeader(Name = "statistics")] string statistics, [FromHeader(Name = "x-requestid")] Guid requestId)
    {

        var updateRedditCommand = new UpdateRedditCommand(requestId, subreddit, statistics.Split(","));
        service.Logger.LogInformation(
                  "Sending command: {CommandName} - {SubRedditName}: {Statistics} ({@Command})",
                  updateRedditCommand.GetType(),
                  nameof(updateRedditCommand.list),
                  updateRedditCommand.statistics,
                  updateRedditCommand);
     
        var commandResult = await service.Mediator.Send(updateRedditCommand);

        service.Logger.LogInformation($"RequestId {requestId}. Complete publish of Subreddit: {subreddit} for {statistics} to load from api and save to cache");

        return TypedResults.Ok(commandResult);

    }

}
