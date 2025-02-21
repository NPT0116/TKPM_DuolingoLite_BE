using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Configures.Query.GetAConfigure;

public class GetAConfigureQueryHandler: IQueryHandler<GetAConfigureQuery, ConfigureResponseDto>
{
    private readonly IConfigurationRepository _configurationRepository;
    public GetAConfigureQueryHandler(IConfigurationRepository configurationRepository   )
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<Result<ConfigureResponseDto>> Handle(GetAConfigureQuery request, CancellationToken cancellationToken)
    {
        var configure = await _configurationRepository.GetConfigureById(request.ConfigureId);
        if (configure is null)
        {
            return Result.Failure<ConfigureResponseDto>(ConfigurationError.ConfigurationNotFound(request.ConfigureId));
        }
        return new ConfigureResponseDto(
            configure.Id,
            configure.Audio,
            configure.Image,
            configure.EnglishText,
            configure.VietnameseText
        );
    }
}

