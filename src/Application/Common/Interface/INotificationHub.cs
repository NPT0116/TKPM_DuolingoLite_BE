using Domain.Entities;

namespace Application.Interfaces;

public interface INotificationHub
{
    Task SendNotificationToUser(string userId, object notification);
} 