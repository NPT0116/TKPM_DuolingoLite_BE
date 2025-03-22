using Domain.Entities;
using MediatR;

namespace Application.Notifications.Commands.SendNotification;

public record SendNotificationCommand : IRequest<Unit>
{
    public string UserId { get; init; } = string.Empty;
    public NotificationType NotificationType { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
} 