using API.Models;
using Microsoft.EntityFrameworkCore;
using API.LIB.INTERFACES;
using API.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using API.LIB.HELPERS;

namespace API.Data.REPOSITORY;

public class MessageRepository : IMessageRepository
{

    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
            _mapper = mapper;
            _context = context;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(message => message.MessageSent).AsQueryable();
        query = messageParams.Container switch{
            "Inbox" => query.Where(user => user.RecipientUsername == messageParams.Username && user.RecipientDeleted == false),
            "Outbox" => query.Where(user => user.SenderUsername == messageParams.Username && user.SenderDeleted == false),
            _ => query.Where(user => user.RecipientUsername == messageParams.Username && user.RecipientDeleted == false && user.DateRead == null),
        };
        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);
        
        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(user => user.Sender).ThenInclude(user => user.Photos)
            .Include(user => user.Recipient).ThenInclude(user => user.Photos)
            .Where(
                message => 
                message.RecipientUsername == currentUsername && message.RecipientDeleted == false 
                && message.SenderUsername == recipientUsername || 
                message.RecipientUsername == recipientUsername && message.SenderDeleted == false 
                && message.SenderUsername == currentUsername
            ).OrderBy(message => message.MessageSent).ToListAsync();
        
        var unreadMessages = messages.Where(message => message.DateRead == null && message.RecipientUsername == currentUsername).ToList();
        if(unreadMessages.Any()){
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }           


    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}
