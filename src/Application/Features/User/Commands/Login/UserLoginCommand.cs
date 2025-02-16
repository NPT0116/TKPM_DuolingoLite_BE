using System;
using System.Windows.Input;
using Application.Abstractions.Messaging;

namespace Application.Features.User.Commands.Login;


public record UserLoginRequestDto(string Email, string Password);


public record UserLoginCommand(UserLoginRequestDto UserLoginRequestDto): ICommand<string>;


