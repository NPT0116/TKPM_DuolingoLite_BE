using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Courses;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        Task<Course?> GetCourseById(Guid id);
        Task<Course> CreateCourse(Course course);
        Task<List<Course>> GetAllCourses(CancellationToken cancellationToken);
        Task<int> GetCourseCount();
        Task<Course?> GetCourseByLevel(int level);
        Task<Course?> GetCourseByName(string name);
    }
}