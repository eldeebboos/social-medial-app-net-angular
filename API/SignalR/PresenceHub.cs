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

        var isOnline = await presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserName());

        var currentusers = await presenceTracker.getOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentusers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null) throw new HubException("Cannot get user claim");

        var isOffline = await presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());



        await base.OnDisconnectedAsync(exception);
    }
}
