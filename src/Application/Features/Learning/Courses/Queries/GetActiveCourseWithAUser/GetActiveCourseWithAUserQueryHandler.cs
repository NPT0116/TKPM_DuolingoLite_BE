using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;

public class GetActiveCourseWithAUserQueryHandler : IQueryHandler<GetActiveCourseWithAUserQuery, GetActiveCourseWithAUserResponseDto>
{
    private readonly  ILearningProgressRepository _learningProgressRepository;
<<<<<<< HEAD
    public GetActiveCourseWithAUserQueryHandler(ILearningProgressRepository learningProgressRepository)
    {
        _learningProgressRepository = learningProgressRepository;
=======
    private readonly ICourseRepository _courseRepository;
    public GetActiveCourseWithAUserQueryHandler(ILearningProgressRepository learningProgressRepository, ICourseRepository courseRepository)
    {
        _learningProgressRepository = learningProgressRepository;
        _courseRepository = courseRepository;
>>>>>>> 1878263021726ce834ebad7c126332daa7d06736
    }
    public async Task<Result<GetActiveCourseWithAUserResponseDto>> Handle(GetActiveCourseWithAUserQuery request, CancellationToken cancellationToken)
    {
        var lp  = await _learningProgressRepository.GetLearningProgressByUserIdAsync(request.UserId);
        if(lp == null)
        {
            return Result.Failure<GetActiveCourseWithAUserResponseDto>(LearningProgressError.LearningProgresssForUserNotFound(request.UserId));
        }
        var returnBody = new GetActiveCourseWithAUserResponseDto(
            lp.Course.Id,
            lp.LessonOrder,
            lp.UserId
        );
        return returnBody;
    }
}
