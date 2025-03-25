using System;
using System.Windows.Input;
using Application.Abstractions.Messaging;

namespace Application.Features.User.Commands.UpgradeUser;


public record UpgradeUserCommandDto
{
    public int DurationInMonth { get; init; }
    public long Price { get; init; }
}
public record UpgradeUserCommand(UpgradeUserCommandDto UpgradeUserCommandDto): ICommand<bool>;
