using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILearningProgressRepository _learningProgressRepository;
        public DeleteCourseCommandHandler(
            ICourseRepository courseRepository,
            ILearningProgressRepository learningProgressRepository)
        {
            _courseRepository = courseRepository;
            _learningProgressRepository = learningProgressRepository;
        }

        public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var courseToDelete = await _courseRepository.GetCourseById(request.courseId);
            if(courseToDelete == null)
            {
                return Result.Failure(CourseError.CourseNotFound(request.courseId));
            }

            var userFollowingCourse = await _learningProgressRepository.GetUserCountRegisteringCourse(request.courseId);
            if(userFollowingCourse > 0)
            {
                return Result.Failure(CourseError.CourseIsBeingFollowedByUser);
            }

            await _courseRepository.DeleteCourse(courseToDelete);
            return Result.Success();
        }
    }
}