
using System.Text.RegularExpressions;
using API.DTO;
using API.EXTENSIONS;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Group = API.Models.Group;

namespace API.CHAT;

[Authorize]
public class MessageHub: Hub{

    private  readonly IMessageRepository messageRepository;
    private  readonly IUserRepository userRepository;
    private  readonly IMapper mapper;
    private  readonly IHubContext<PresenceHub> presenceHub;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub){
        this.messageRepository = messageRepository;
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.presenceHub = presenceHub;
    }

    public override async Task OnConnectedAsync()
    {
        var httpcontext = Context.GetHttpContext();
        var otherUser = httpcontext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
 
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
        await Clients.Caller.SendAsync("RecieveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception){
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO){
        var username = Context.User.GetUsername();
        if(username == createMessageDTO.RecipientUsername.ToLower())
            throw new HubException("You cannot send message to yourself.");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);
        if(recipient == null) throw new HubException("Not found user");

        var message = new Message{
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRepository.GetMessageGroup(groupName);
        if(group.Connections.Any(user => user.Username == recipient.UserName)){
            message.DateRead = DateTime.UtcNow;
        }else{
            var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
            if(connections != null){
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        messageRepository.AddMessage(message);
        if(await messageRepository.SaveAllAsync()){
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
        }
    }
 
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName){
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if(group == null){
            group = new Group(groupName);
            messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        if(await messageRepository.SaveAllAsync()) return group;
        
        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup(){
        var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

        messageRepository.RemoveConnection(connection);
        if(await messageRepository.SaveAllAsync()) return group;

        throw new HubException("Failed to remove from group");
    }
}