using Microsoft.AspNetCore.SignalR;
using Domain.Entities;
using Application.Interfaces;
using System.Security.Claims;

namespace Infrastructure.Hubs
{
    public class NotificationHub : Hub, INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITokenService _tokenService;

        public NotificationHub(
            IHubContext<NotificationHub> hubContext,
            ITokenService tokenService)
        {
            _hubContext = hubContext;
            _tokenService = tokenService;
        }

        public async Task SendNotificationToUser(string userId, object notification)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var accessToken = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(accessToken))
                {
                    Context.Abort();
                    return;
                }

                var userId = _tokenService.GetUserIdFromToken(accessToken);
                if (string.IsNullOrEmpty(userId))
                {
                    Context.Abort();
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                var welcomeMessage = new 
                { 
                    title = "Welcome", 
                    message = "You are connected to the hub!" 
                };

                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", welcomeMessage);
                await base.OnConnectedAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var accessToken = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                var userId = !string.IsNullOrEmpty(accessToken) ? _tokenService.GetUserIdFromToken(accessToken) : null;

                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
