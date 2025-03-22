using System;
using Application.Abstractions.Messaging;

namespace Application.Features.User.Queries.GetUserActivityRange;


public record GetUserAcitivityRangeQueryParam ( DateTime StartDate, DateTime EndDate);
public record GetUserActivityDto (Guid UserId, DateTime Date, Guid Id);

public record GetUserAcitivityRangeQuery(GetUserAcitivityRangeQueryParam GetUserAcitivityRangeQueryParam) : IQuery<List<GetUserActivityDto>>;
