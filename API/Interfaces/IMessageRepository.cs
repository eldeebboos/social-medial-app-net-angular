using System;
using API.Entities;
using API.Helpers;
using API.DTOs;

namespace API.Interfaces;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection?> GetConnection(string connectionId);
    Task<Group?> GetMessageGroup(string groupName);
    Task<Group?> GetGroupForConnection(string connectionId);

}
