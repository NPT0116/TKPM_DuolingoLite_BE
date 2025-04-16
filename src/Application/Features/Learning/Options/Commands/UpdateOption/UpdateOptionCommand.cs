using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Options;

namespace Application.Features.Learning.Options.Commands.UpdateOption;

public record UpdateOptionDto(
    string? vietnameseText,
    string? image,
    string? audio,
    string? englishText,
    bool needAudio,
    bool needImage
);

public record UpdateOptionCommand(
    Guid optionId,
    UpdateOptionDto optionDto
) : ICommand<Option>;