using Microsoft.AspNetCore.SignalR;
using Domain.Entities;
using Application.Interfaces;
using System.Security.Claims;
using System.Collections.Concurrent;
using Application.Common.Utls;

namespace Infrastructure.Hubs
{
    public class NotificationHub : Hub, INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ITokenService _tokenService;
        
        // ConcurrentDictionary để lưu trữ các kết nối theo userId
        public static ConcurrentDictionary<string, List<string>> ConnectedUsers = new();

        public NotificationHub(
            IHubContext<NotificationHub> hubContext,
            ITokenService tokenService)
        {
            _hubContext = hubContext;
            _tokenService = tokenService;
        }

        public async Task SendNotificationToUser(string userId, object notification)
        {
            // Gửi thông báo tới group userId (những connection đã được thêm vào group)
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var accessToken = httpContext?.Request.Query["access_token"].ToString();

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

                // Thêm connection vào group dựa theo userId
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                // Cập nhật ConnectedUsers
                ConnectedUsers.AddOrUpdate(
                    userId,
                    new List<string> { Context.ConnectionId },
                    (key, existingList) =>
                    {
                        existingList.Add(Context.ConnectionId);
                        return existingList;
                    });

                // Gửi thông báo chào mừng cho connection mới
                var welcomeMessage = new 
                { 
                    title = "Welcome", 
                    message = "You are connected to the hub!" 
                };

                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotification", welcomeMessage);
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                // Bạn có thể log exception tại đây nếu cần
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var accessToken = httpContext?.Request.Query["access_token"].ToString();
                var userId = !string.IsNullOrEmpty(accessToken) ? _tokenService.GetUserIdFromToken(accessToken) : null;

                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

                    // Cập nhật ConnectedUsers: loại bỏ connection của user đang ngắt kết nối
                    if (ConnectedUsers.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(Context.ConnectionId);
                        if (!connections.Any())
                        {
                            ConnectedUsers.TryRemove(userId, out _);
                        }
                    }
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                // Bạn có thể log exception tại đây nếu cần
                throw;
            }
        }

        // Ví dụ gửi thông báo ôn tập review cho user
        public Task ReviewNotification(string userId, Notification review)
        {
            PrintUtils.PrintAsJson(review);
            return SendNotificationToUser(userId, review);
        }

        // Ví dụ gửi thông báo thanh toán cho user
        public Task PaymentNotification(string userId, Notification review)
        {
            return SendNotificationToUser(userId, review);
        }
    }
}
