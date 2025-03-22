using Application.Common.Utls;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string userId, NotificationType type, string title, string message)
    {
        var notification = Notification.Create(userId, type, title, message);
        // PrintUtils.PrintAsJson(notification);
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            notification.NotificationId,
            notification.UserId,
            notification.NotificationType,
            notification.Title,
            notification.Message,
            notification.CreatedAt,
            notification.IsRead
        });
    }
} 