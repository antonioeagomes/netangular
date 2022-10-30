using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public MessageHub(IUnityOfWork unityOfWork, IMapper mapper)
    {
        _unityOfWork = unityOfWork;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();

        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _unityOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
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

        _unityOfWork.MessageRepository.AddMessage(message);

        if (await _unityOfWork.Complete()) 
        {
            var group = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}