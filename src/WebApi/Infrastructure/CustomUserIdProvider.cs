using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Application.Interfaces;

namespace WebApi.Infrastructure;

public class CustomUserIdProvider : Application.Interfaces.IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
} 