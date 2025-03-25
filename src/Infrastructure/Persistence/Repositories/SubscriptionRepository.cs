using System;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context;
    public SubscriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool RemoveSubscription(Guid SubscriptionId)
    {
        var subscription = _context.Subscriptions.Find(SubscriptionId);
        if (subscription is not null)
        {
            _context.Subscriptions.Remove(subscription);
            return true;
        }
        return false;
    }
}
