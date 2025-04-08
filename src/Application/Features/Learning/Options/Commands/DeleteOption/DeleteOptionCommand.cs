

using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Options.Commands.DeleteOption;

public record DeleteOptionCommand(Guid optionId) : ICommand;