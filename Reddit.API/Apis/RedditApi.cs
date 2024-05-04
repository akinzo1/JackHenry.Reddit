
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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


    public static async Task<Results<Ok, BadRequest<string>>> UpdateReddit([AsParameters] RedditService service, [FromHeader(Name = "x-requestid")] Guid requestId)
    {

        //var cardTypes = await orderQueries.GetCardTypesAsync();
        service.Logger.LogInformation("hitting api");
        var updateRedditCommand = new UpdateRedditCommand("funny");
        
        var commandResult = await service.Mediator.Send(updateRedditCommand);//.GetString();
        
        service.Logger.LogInformation($"logged {commandResult} to cache");

        return TypedResults.Ok();


    }

    private static async Task<string> GetString()
    {
        return await Task.FromResult("This string");
    }

}
