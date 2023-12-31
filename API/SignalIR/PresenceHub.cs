
using API.EXTENSIONS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.CHAT;

[Authorize]
public class PresenceHub: Hub{

    private readonly PresenceTracker presenceTracker;
    public PresenceHub(PresenceTracker presenceTracker){
        this.presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync(){
        var isOnline = await presenceTracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception){

        var isOffline = await presenceTracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
        if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
//GetOnlineUsers


        await base.OnDisconnectedAsync(exception);    
    }        


}