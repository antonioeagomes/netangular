using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Backend.DTO;
using Backend.Entities;
using Backend.Extensions;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(MessageToCreateDTO messageToCreate)
    {
        var username = User.GetUsername();

        if (username == messageToCreate.RecipientUsername) return BadRequest("You cannnot send a message to yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(messageToCreate.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message()
        {
            Content = messageToCreate.Content,
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

        return BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpGet("thread/{targetUsername}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string targetUsername)
    {
        var currentUsername = User.GetUsername();
        var messages = await _messageRepository.GetMessageThread(currentUsername, targetUsername);

        return Ok(messages);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _messageRepository.GetMessage(id);

        if (message.Sender.UserName != username && message.Recipient.UserName != username) return Unauthorized();
        
        if (message.Sender.UserName == username) message.SenderDeleted = true;

        if (message.Recipient.UserName == username) message.RecipientDeleted = true;

        if(message.RecipientDeleted && message.SenderDeleted) _messageRepository.DeleteMessage(message);

        if(await _messageRepository.SaveAllAsync()) return Ok();

        return BadRequest();
    }

}
