using Application.Abstractions.Messaging;
using Application.Features.User.Commands.Common;
using System;

namespace Application.Features.User.Commands.Register;

public record UserRegisterDto (string FirstName, string LastName, string Email , string UserName, string Password);
public record UserRegisterCommand(UserRegisterDto UserRegisterDto, AvatarUploadRequest? AvatarUploadRequest): ICommand<Guid>;
