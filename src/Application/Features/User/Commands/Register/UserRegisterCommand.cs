using Application.Abstractions.Messaging;
using System;

namespace Application.Features.User.Commands.Register;

public record AvatarUploadRequest(byte[] FileData, string FileName, string ContentType);
public record UserRegisterDto (string FirstName, string LastName, string Email , string UserName, string Password);
public record UserRegisterCommand(UserRegisterDto UserRegisterDto, AvatarUploadRequest? AvatarUploadRequest): ICommand<Guid>;
