using MediatR;

namespace Application.Features.Learning.Words.Queries.GetWordDefinition;

public record WordDefinitionDto(
    string Word,
    string? Phonetic,
    List<PhoneticDto> Phonetics,
    string? Origin,
    List<MeaningDto> Meanings
);

public record PhoneticDto(
    string? Text,
    string? Audio
);

public record MeaningDto(
    string? PartOfSpeech,
    List<DefinitionDto> Definitions
);

// Assuming a DefinitionDto with these properties as an example:
public record DefinitionDto(
    string? DefinitionText,
    string? Example
);

public record GetWordDefinitionQuery(string Word) : IRequest<List<WordDefinitionDto>>;