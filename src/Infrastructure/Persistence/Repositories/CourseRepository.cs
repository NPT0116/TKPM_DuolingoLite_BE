using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Course> CreateCourse(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<List<Course>> GetAllCourses(CancellationToken cancellationToken)
        {
            return await _context
                .Courses
                .Include(c => c.Lessons)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetCourseById(Guid id)
        {
            return await _context.Courses.FindAsync(id);
        }
    }
}