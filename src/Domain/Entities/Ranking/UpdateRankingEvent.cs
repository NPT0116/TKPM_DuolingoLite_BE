using System;
using MediatR;

namespace Domain.Entities.Ranking;

public record UpdateRankingEvent(int ExperiencePoint, Guid UserId): INotification;

    

