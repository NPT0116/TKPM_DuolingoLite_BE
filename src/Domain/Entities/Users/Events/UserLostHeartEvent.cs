using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Users.Events;

public record UserLostHeartEvent(Guid userId) : IDomainEvent;