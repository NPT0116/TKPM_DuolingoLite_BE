using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SharedKernel;

namespace Application.Features.Heart.Commands.LoseHeart;

public record UserHeartDto(
    Guid userId,
    int remainingHearts
);

public record LoseHeartCommand(Guid userId) : IRequest<Result<UserHeartDto>>;