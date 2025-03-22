using Microsoft.AspNetCore.SignalR;

namespace Application.Interfaces;

public interface IUserIdProvider
{
    string? GetUserId(HubConnectionContext connection);
} 