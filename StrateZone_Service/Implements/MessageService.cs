using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task<MessageModel> SendMessageAsync(MessageRequest request)
        {
            try
            {
                if (request.SenderId == null || request.ReceiverId == null) throw new Exception("Sender and receiver id can not be null.");

                MessageModel model = new()
                {
                    SenderId = request.SenderId,
                    ReceiverId = request.ReceiverId,
                    Content = request.Content,
                    Status = StrateZone_Repository.Parameters.PostgreEnums.MessageStatus.unread,
                    CreatedAt = DateTime.UtcNow,
                };

                var message = _mapper.Map<Message>(model);
                var result = await _messageRepository.SendMessageAsync(message);

                return _mapper.Map<MessageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MessageModel>> GetMessagesFromUserIdAsync(int id)
        {
            try
            {
                var result = await _messageRepository.GetMessagesFromUserIdAsync(id);
                return _mapper.Map<List<MessageModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MessageModel>> GetMessagesFromSenderIdToReceiverIdAsync(int sender_id, int receiver_id)
        {
            try
            {
                var result = await _messageRepository.GetMessagesFromSenderIdToReceiverIdAsync(sender_id, receiver_id);
                return _mapper.Map<List<MessageModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MessageModel>> GetConversationMessagesAsync(int user_1_id, int user_2_id)
        {
            try
            {
                var result = await _messageRepository.GetConversationMessagesAsync(user_1_id, user_2_id);
                return _mapper.Map<List<MessageModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
