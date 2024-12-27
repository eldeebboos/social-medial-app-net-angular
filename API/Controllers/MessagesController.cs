using System;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateMessage([FromBody] CreateMessageDto createMessageDto)
    {
        var username = User.GetUserName();

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            return BadRequest("Cannot send this message");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            content = createMessageDto.Content,

        };

        unitOfWork.MessageRepository.Add(message);

        if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDto>(message));
        return BadRequest("Cannot save messages now");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUserName();
        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
        Response.AddPageinationHeader(messages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
    {
        var currentUsername = User.GetUserName();
        return Ok(await unitOfWork.MessageRepository.GetMessagesThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessages(int id)
    {
        var username = User.GetUserName();
        var message = await unitOfWork.MessageRepository.GetMessage(id);
        if (message == null) return BadRequest("Cannot delete this message");

        if (message.SenderUsername != username && message.RecipientUsername != username)
            return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
            unitOfWork.MessageRepository.Delete(message);

        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("Cannot delete message");
    }
}
