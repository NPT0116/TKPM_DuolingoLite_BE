using Domain.Entities;

namespace Application.Interfaces;

public interface INotificationHub
{
    Task SendNotificationToUser(string userId, object notification);
    Task ReviewNotification(string userId, Notification review);
    Task PaymentNotification(string userId, Notification review);
} 