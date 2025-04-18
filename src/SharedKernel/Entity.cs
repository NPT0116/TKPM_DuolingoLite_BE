﻿using System.Text.Json.Serialization;

namespace SharedKernel;

public abstract class Entity
{
    public Guid Id { get; init; }
    
    private readonly List<IDomainEvent> _domainEvents = [];
    [JsonIgnore]
    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
