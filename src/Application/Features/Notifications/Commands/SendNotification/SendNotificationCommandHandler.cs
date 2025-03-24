using Application.Common.Utls;
using Application.Interfaces;
using MediatR;

namespace Application.Notifications.Commands.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
{
    private readonly INotificationService _notificationService;

    public SendNotificationCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.SendNotificationAsync(
            request.UserId,
            request.NotificationType,
            request.Title,
            request.Message);
        // PrintUtils.PrintAsJson(request); // Fixed the method call to use the Print method correctly

        return Unit.Value;
    }
} 