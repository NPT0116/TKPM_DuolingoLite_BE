using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Queries.GetCourseList
{
    public class GetCourseListQueryHandler : IQueryHandler<GetCourseListQuery, List<Course>>
    {
        private readonly ICourseRepository _courseRepository;
        public GetCourseListQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task<Result<List<Course>>> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllCourses(cancellationToken);
            return courses;
        }
    }
}