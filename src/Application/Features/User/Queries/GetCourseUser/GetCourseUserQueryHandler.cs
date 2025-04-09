using System;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.User.Queries.GetCourseUser;

public class GetCourseUserQueryHandler: IQueryHandler<GetCourseUserQuery, List<GetCourseUserQueryResponseDto>>
{
    private readonly ILearningProgressRepository _learningProgressRepository;
    private readonly IIdentityService   _identityService;
    public GetCourseUserQueryHandler(IIdentityService identityService, ILearningProgressRepository learningProgressRepository)
    {
        _identityService = identityService;
        _learningProgressRepository = learningProgressRepository;        
    }

    public async Task<Result<List<GetCourseUserQueryResponseDto>>> Handle(GetCourseUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetCurrentUserAsync();
        if (user == null)
        {
            return Result.Failure<List<GetCourseUserQueryResponseDto>>(UserError.Unauthorized());
        }
        var learningProgress = await _learningProgressRepository.GetCoursesByUserIdAsync(user.Id);

        var result = learningProgress.Select(x => new GetCourseUserQueryResponseDto(
            x.Course.Id,
            x.LessonOrder,
            x.Course.Lessons.Count,
            x.Course.Name
        )).ToList();

        return result;
    }
}
