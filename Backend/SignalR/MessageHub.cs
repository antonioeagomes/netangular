using AutoMapper;
using Backend.DTO;
using Backend.Entities;
using Backend.Extensions;
using Backend.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Backend.SignalR;

public class MessageHub : Hub
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;

    public MessageHub(IUnityOfWork unityOfWork, IMapper mapper, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
    {
        _unityOfWork = unityOfWork;
        _mapper = mapper;
        _presenceHub = presenceHub;
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();

        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await _unityOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(MessageToCreateDTO messageToCreate)
    {
        var username = Context.User.GetUsername();

        if (username == messageToCreate.RecipientUsername) throw new HubException("You cannnot send a message to yourself");

        var sender = await _unityOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unityOfWork.UserRepository.GetUserByUsernameAsync(messageToCreate.RecipientUsername);

        if (recipient == null || sender == null) throw new HubException();

        var message = new Message()
        {
            Content = messageToCreate.Content,
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _unityOfWork.MessageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(u => u.Username == recipient.UserName))
        {
            message.ReadAt = DateTime.UtcNow;
        }
        else
        {
            var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = sender.UserName, knownAs = sender.KnownAs });
            }
        }

        _unityOfWork.MessageRepository.AddMessage(message);

        if (await _unityOfWork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _unityOfWork.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _unityOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _unityOfWork.Complete();

    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _unityOfWork.MessageRepository.GetConnection(Context.ConnectionId);
        _unityOfWork.MessageRepository.RemoveConnection(connection);
        await _unityOfWork.Complete();
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;

        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}