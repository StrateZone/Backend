using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Hubs;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger, IHubContext<ChatHub> chatHub)
        {
            _messageService = messageService;
            _logger = logger;
            _chatHub = chatHub;
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetConversation(int id)
        {
            try
            {
                var messages = await _messageService.GetMessagesFromUserIdAsync(id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{senderId}/to/{receiverId}")]
        public async Task<IActionResult> GetMessagesFromSenderToReceiver(int senderId, int receiverId)
        {
            try
            {
                var messages = await _messageService.GetMessagesFromSenderIdToReceiverIdAsync(senderId, receiverId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("users/{user1_id}/and/{user2_id}")]
        public async Task<IActionResult> GetConversation(int user1_id, int user2_id)
        {
            try
            {
                if (user1_id <= 0 || user2_id <= 0)
                    return BadRequest("Sender and Receiver ID can not be null");

                var messages = await _messageService.GetConversationMessagesAsync(user1_id, user2_id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            try
            {
                if (request.SenderId == null || request.ReceiverId == null) 
                    return BadRequest("Sender and Receiver ID can not be null");

                await _chatHub.Clients.All.SendAsync("ReceiveMessage", request.SenderId, request.ReceiverId, request.Content);

                var message = await _messageService.SendMessageAsync(request);
                return Created("Message sent", message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
