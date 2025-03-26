using System;
using SharedKernel;

namespace Domain.Entities.Users.Events;

public record UserPaymentEvent(Guid UserId): IDomainEvent;