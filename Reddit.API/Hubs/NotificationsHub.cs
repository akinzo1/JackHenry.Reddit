using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using static Reddit.API.Model.Api.SubRedditApiResponse;

namespace Reddit.API;
public class NotificationsHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task RedditUpVotesUpdated(List<SubReddit> upVotes, string name, DateTime createdDate)
    {
        await Clients.All.SendAsync("RedditUpVotesUpdated", upVotes, name, createdDate);
    }

    public async Task RedditMostPostsUpdated(List<UserCounts> userCounts)
    {
        await Clients.All.SendAsync("RedditMostPostsUpdated", userCounts);
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "RedditAPI");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RedditAPI");
        await base.OnDisconnectedAsync(ex);
    }
}
