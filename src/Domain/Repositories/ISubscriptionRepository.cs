using System;

namespace Domain.Repositories;

public interface ISubscriptionRepository
{
    public  bool RemoveSubscription(Guid SubscriptionId) ;
}
