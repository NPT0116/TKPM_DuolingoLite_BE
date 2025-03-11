using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;

namespace Application.Features.Heart.Queries.GetUserHeart;

public record UserHeartResponseDto(
    Guid userId,
    int remainingHearts
);

public record GetUserHeartQuery(Guid userId) : IRequest<Result<UserHeartResponseDto>>;