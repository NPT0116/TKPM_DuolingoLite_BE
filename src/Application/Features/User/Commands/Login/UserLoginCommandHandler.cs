using System;
using Application.Abstractions.Messaging;
using Application.Interface;
using SharedKernel;

namespace Application.Features.User.Commands.Login;

public class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, string>
{
    private readonly IIdentityService _identityService;
    public UserLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {

        var (result, token) = await _identityService.LoginAsync(request.UserLoginRequestDto.Email, request.UserLoginRequestDto.Password);

        if (result.IsFailure)
        {
            return result.Error;
        }

        return token;
    }


}
