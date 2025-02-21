using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Configures.Query.GetAConfigure;


public record ConfigureResponseDto (Guid Id, bool Audio, bool Image, bool EnglishText, bool VietnameseText);
public record GetAConfigureQuery(Guid ConfigureId): IQuery<ConfigureResponseDto>;