using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;

namespace StrateZone_Service.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageModel>> GetConversationMessagesAsync(int user_1_id, int user_2_id);
        Task<List<MessageModel>> GetMessagesFromSenderIdToReceiverIdAsync(int sender_id, int receiver_id);
        Task<List<MessageModel>> GetMessagesFromUserIdAsync(int id);
        Task<MessageModel> SendMessageAsync(MessageRequest request);
    }
}