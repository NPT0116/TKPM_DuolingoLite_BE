using System;
using Application.Common.Utls;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Users.Events;
using MediatR;

namespace Infrastructure.EventHandlers;

public class UserPaymentEventHandler : INotificationHandler<UserPaymentEvent>
{
    private readonly INotificationHub _notification;
    public UserPaymentEventHandler(INotificationHub notification)
    {
        _notification = notification;
    }
    public Task Handle(UserPaymentEvent notification, CancellationToken cancellationToken)
    {
        var noti = Notification.Create( notification.UserId.ToString() , NotificationType.PaymentNoti, "Thanh toán", "Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi");
        // PrintUtils.PrintAsJson(noti);
        _notification.PaymentNotification(notification.UserId.ToString(), noti);
        return Task.CompletedTask;
    }
}