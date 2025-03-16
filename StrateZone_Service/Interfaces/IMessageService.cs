using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;

namespace StrateZone_Service.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageResponse>> GetConversationMessagesAsync(int user_1_id, int user_2_id);
        Task<List<MessageResponse>> GetMessagesFromSenderIdToReceiverIdAsync(int sender_id, int receiver_id);
        Task<List<MessageResponse>> GetMessagesFromUserIdAsync(int id);
        Task<MessageModel> SendMessageAsync(MessageRequest request);
    }
}