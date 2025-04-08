using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.DeleteLesson
{
    public class DeleteLessonCommandHandler : ICommandHandler<DeleteLessonCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILearningProgressRepository _learningProgressRepository;
        private readonly IApplicationDbContext _context;
        public DeleteLessonCommandHandler(
            ICourseRepository courseRepository,
            ILearningProgressRepository learningProgressRepository,
            IApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _learningProgressRepository = learningProgressRepository;
            _context = context;
        }

        async Task<Result> IRequestHandler<DeleteLessonCommand, Result>.Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseById(request.courseId);
            if(course == null)
            {
                return Result.Failure(CourseError.CourseNotFound(request.courseId));
            }

            var lessonCount = course.Lessons.Count();
            if(lessonCount == 0) return Result.Failure(CourseError.NoLessonInCourse);
            
            var lesson = course.GetLessonByOrder(request.lessonOrder);
            if(lesson == null)
            {
                return Result.Failure(CourseError.QuestionWithOrderNotFound(request.lessonOrder, request.courseId));
            }

            var enrolledUserCount = await _learningProgressRepository.GetEnrolledUserCountForLessonAsync(request.courseId, request.lessonOrder);
            if(enrolledUserCount > 0)
            {
                return Result.Failure(LessonError.LessonHasBeenLearnedByUser(request.courseId, request.lessonOrder));
            }

            var userTakingCourseCount = await _courseRepository.GetUserTakingCourseCount(request.courseId);
            if(userTakingCourseCount > 0)
            {
                return Result.Failure(CourseError.CannotAddLessonToTakenCourse(request.courseId));
            }

            course.RemoveLesson(lesson);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}