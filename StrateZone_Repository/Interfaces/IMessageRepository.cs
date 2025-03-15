using StrateZone_Repository.Entities;

namespace StrateZone_Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> DeleteMessageAsync(int id);
        Task<List<Message>> GetConversationMessagesAsync(int user_1_Id, int user_2_Id);
        Task<List<Message>> GetMessagesFromSenderIdToReceiverIdAsync(int senderId, int receiverId);
        Task<List<Message>> GetMessagesFromUserIdAsync(int id);
        Task<Message> SendMessageAsync(Message message);
    }
}