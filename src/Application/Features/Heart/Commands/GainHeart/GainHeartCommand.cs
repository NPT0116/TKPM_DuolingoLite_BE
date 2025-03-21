using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Dtos;
using MediatR;
using SharedKernel;

namespace Application.Features.Heart.Commands.GainHeart;

public record GainHeartCommand(Guid userId) : IRequest<Result<UserHeartDto>>;