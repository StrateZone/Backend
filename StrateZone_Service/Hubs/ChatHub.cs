using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace StrateZone_Service.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            try
            {
                _logger.LogInformation("Attempting to send message from {SenderId} to {ReceiverId}", senderId, receiverId);
                await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, message);
                _logger.LogInformation("Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
