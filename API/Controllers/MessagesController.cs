using System.Security.Claims;
using API.Data;
using API.DTO;
using API.EXTENSIONS;
using API.LIB.HELPERS;
using API.LIB.INTERFACES;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUserRepository userRepository;
    private readonly IMessageRepository messageRepository;
    private readonly IMapper mapper;

    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper){
        this.userRepository = userRepository;
        this.messageRepository = messageRepository;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO){
        var username = User.GetUsername();
        if(username == createMessageDTO.RecipientUsername.ToLower())
            return BadRequest("You can't send messages to yourself !");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);
        if(recipient == null) return NotFound();

        var message = new Message{
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        messageRepository.AddMessage(message);
        if(await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDTO>(message));
        return BadRequest("Failed to send message."); 
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams){
        
        messageParams.Username = User.GetUsername();
        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username){
        
        var currentUsername = User.GetUsername();
        return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id){
        
        var username = User.GetUsername();
        var message = await messageRepository.GetMessage(id);

        if(message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();
        if(message.SenderUsername == username) message.SenderDeleted = true;
        if(message.RecipientUsername == username) message.RecipientDeleted = true;
        if(message.RecipientDeleted && message.SenderDeleted){
            messageRepository.DeleteMessage(message);
        }

        if(await messageRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem deleting message");
    }
}
