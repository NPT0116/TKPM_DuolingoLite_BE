using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Options;

namespace Application.Features.Learning.Options.Commands.CreateOption;

public record CreateOptionDto(
    string? vietnameseText,
    string? image,
    string? audio,
    string? englishText,
    bool needAudio
);

public record CreateOptionCommand(CreateOptionDto dto) : ICommand<Option>;