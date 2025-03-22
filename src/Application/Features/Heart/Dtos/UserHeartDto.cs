using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Heart.Dtos;

public record UserHeartDto(Guid userId, int remainingHearts);