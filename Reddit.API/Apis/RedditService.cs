using MediatR;


public class RedditService(IMediator mediator, ILogger<RedditService> logger)
{
    public IMediator Mediator { get; set; } = mediator;
    public ILogger<RedditService> Logger { get; } = logger;

}
