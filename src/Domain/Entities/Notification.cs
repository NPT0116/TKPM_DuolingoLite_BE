using System;

namespace Domain.Entities;

public class Notification
{
    public Guid NotificationId { get; private set; }
    public string UserId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    private Notification() { } // For EF Core

    private Notification(
        Guid notificationId,
        string userId,
        NotificationType notificationType,
        string title,
        string message)
    {
        NotificationId = notificationId;
        UserId = userId;
        NotificationType = notificationType;
        Title = title;
        Message = message;
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
    }

    public static Notification Create(
        string userId,
        NotificationType notificationType,
        string title,
        string message)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        return new Notification(
            Guid.NewGuid(),
            userId,
            notificationType,
            title,
            message);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
} 