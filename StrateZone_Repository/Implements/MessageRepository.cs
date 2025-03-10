using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;

namespace StrateZone_Repository.Implements
{
    public class MessageRepository : IMessageRepository
    {
        private readonly StrateZoneDbContext _context;

        public MessageRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetMessagesFromUserIdAsync(int id)
        {
            try
            {
                return await _context.Messages
                    .Where(m => m.SenderId == id)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Message>> GetMessagesFromSenderIdToReceiverIdAsync(int senderId, int receiverId)
        {
            try
            {
                return await _context.Messages
                    .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId)
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /**
         * Retrieves a list of messages between user 1 and user 2, ordered by date and time
        **/
        public async Task<List<Message>> GetConversationMessagesAsync(int user_1_Id, int user_2_Id)
        {
            try
            {
                return await _context.Messages
                                    .Where(m =>
                                        (m.SenderId == user_1_Id && m.ReceiverId == user_2_Id)
                                        ||
                                        (m.SenderId == user_2_Id && m.ReceiverId == user_1_Id))
                                    .OrderBy(m => m.CreatedAt)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            try
            {
                string insertQuery = @"
                        INSERT INTO messages (sender_id, receiver_id, content, status, created_at) 
                        VALUES ({0}, {1}, {2}, {3}::message_status, {4})
                        RETURNING message_id;"; 

                var newMessageId = await _context.Database.ExecuteSqlRawAsync(
                    insertQuery,
                    message.SenderId,
                    message.ReceiverId,
                    message.Content,
                    message.Status.ToString(),
                    message.CreatedAt
                );

                message.MessageId = newMessageId; // Assign the returned ID to the user object
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Message> DeleteMessageAsync(int id)
        {
            try
            {
                var toDelete = await _context.Messages.FindAsync(id) ?? throw new Exception("No message with this ID was found");

                _context.Messages.Remove(toDelete);
                await _context.SaveChangesAsync();

                return toDelete;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
