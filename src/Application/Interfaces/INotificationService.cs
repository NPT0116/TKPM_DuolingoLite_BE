using Domain.Entities;

namespace Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string userId, NotificationType type, string title, string message);
} 