using System.Security.Claims;

namespace Application.Interfaces;

public interface ITokenService
{
    string? GetUserIdFromToken(string token);
    ClaimsPrincipal? ValidateToken(string token);
} 