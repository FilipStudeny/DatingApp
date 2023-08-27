

using API.DTO;
using API.LIB.HELPERS;
using API.Models;
using CloudinaryDotNet.Actions;

namespace API.LIB.INTERFACES;


public interface IMessageRepository
    {
    void AddMessage(Message message);
    void DeleteMessage(Message message);

    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();


    //MESSAGES-REALTIME
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);

}