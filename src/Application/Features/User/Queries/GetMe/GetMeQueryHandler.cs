using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Interface;
using Application.Features.User.Queries.GetMe;
using SharedKernel;
using Domain.Entities.User;
using Domain.Entities.Users;

namespace Application.Features.User.Queries.GetMe;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<UserDto>>
{
    private readonly IIdentityService _identityService;

    public GetMeQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDto>> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _identityService.GetCurrentUserAsync();
        
        if (currentUser is null)
            return Result.Failure<UserDto>(UserError.UnauthorizedUser);

        return currentUser;
    }
}
