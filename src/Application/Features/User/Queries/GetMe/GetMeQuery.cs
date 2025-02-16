using System;
using MediatR;
using SharedKernel;

namespace Application.Features.User.Queries.GetMe;

public record UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public record GetMeQuery : IRequest<Result<UserDto>>;

