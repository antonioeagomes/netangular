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
    private readonly IUnityOfWork _unityOfWork;
    private readonly IMapper _mapper;
    public MessagesController(IUnityOfWork unityOfWork, IMapper mapper)
    {
        _unityOfWork = unityOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(MessageToCreateDTO messageToCreate)
    {
        var username = User.GetUsername();

        if (username == messageToCreate.RecipientUsername) return BadRequest("You cannnot send a message to yourself");

        var sender = await _unityOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unityOfWork.UserRepository.GetUserByUsernameAsync(messageToCreate.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message()
        {
            Content = messageToCreate.Content,
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName
        };

        _unityOfWork.MessageRepository.AddMessage(message);

        if (await _unityOfWork.Complete()) return Ok(_mapper.Map<MessageDTO>(message));

        return BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _unityOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await _unityOfWork.MessageRepository.GetMessage(id);

        if (message.Sender.UserName != username && message.Recipient.UserName != username) return Unauthorized();

        if (message.Sender.UserName == username) message.SenderDeleted = true;

        if (message.Recipient.UserName == username) message.RecipientDeleted = true;

        if (message.RecipientDeleted && message.SenderDeleted) _unityOfWork.MessageRepository.DeleteMessage(message);

        if (await _unityOfWork.Complete()) return Ok();

        return BadRequest();
    }

}
