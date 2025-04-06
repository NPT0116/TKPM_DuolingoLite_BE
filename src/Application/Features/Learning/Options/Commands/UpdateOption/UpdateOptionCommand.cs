using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Options;

namespace Application.Features.Learning.Options.Commands.UpdateOption;

public record UpdateOptionDto(
    string? vietnameseText,
    string? imageUrl,
    string? audioUrl,
    string? englishText,
    bool isAudioGenerated,
    bool isImageGenerated
);

public record UpdateOptionCommand(
    Guid optionId,
    UpdateOptionDto optionDto
) : ICommand<Option>;