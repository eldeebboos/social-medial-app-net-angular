using System;
using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null) throw new HubException("Cannot get user claim");

        await presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserName());

        var currentusers = await presenceTracker.getOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentusers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null) throw new HubException("Cannot get user claim");

        await presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());

        var currentusers = await presenceTracker.getOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentusers);

        await base.OnDisconnectedAsync(exception);
    }
}
