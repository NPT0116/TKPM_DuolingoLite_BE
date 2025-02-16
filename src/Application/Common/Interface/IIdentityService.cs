using System;
using Application.Features.User.Queries.GetMe;
using SharedKernel;

namespace Application.Interface;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(Guid userId);
    Task<bool> UseEmailExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string userName);
    Task<bool> IsInRoleAsync(Guid userId, string role);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    Task<(Result Result, Guid UserId)> CreateUserAsync(string FirstName, string LastName, string Email, string userName, string password);

    Task<Result> DeleteUserAsync(Guid userId);
    Task<(Result Result, string Token)> LoginAsync(string email, string password);

    Task<UserDto?> GetCurrentUserAsync();
}
