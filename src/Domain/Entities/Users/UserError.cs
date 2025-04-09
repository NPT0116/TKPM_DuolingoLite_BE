using System;
using SharedKernel;

namespace Domain.Entities.Users;
 
public static class UserError
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Unauthorized(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");
    public static readonly Error UserNameNotUnique = Error.Conflict(
        "Users.UserNameNotUnique",
        "The provided username is not unique");
    public static readonly Error InvalidPassword = Error.Validation(
        "Users.InvalidPassword",
        "The provided password is invalid");

    public static readonly Error UnauthorizedUser = Error.Validation(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static Error UserProfileNotFound(Guid userId) => Error.NotFound(
        "Users.UserProfileNotFound",
        $"The user profile with the Id = '{userId}' was not found");
    
    public static Error UserStatsNotFound(Guid userId) => Error.NotFound(
        "Users.UserStatsNotFound",
        $"The user stats with the Id = '{userId}' was not found");
}
