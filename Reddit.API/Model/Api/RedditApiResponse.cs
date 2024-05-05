namespace Reddit.API.Model.Api;

public record RedditApiResponse
{
    public required string RedditListName { get; set; }
    public int Remaining { get; set; }
    public int Used { get; set; }
    public int Reset { get; set; }
}