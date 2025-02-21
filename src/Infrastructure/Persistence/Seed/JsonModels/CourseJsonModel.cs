using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Courses;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class CourseJsonModel
    {
        public required string Name { get; set; }
        public int Level { get; set; }
        public Course? NextCourse { get; set; }
        public List<LessonJsonModel> Lessons { get; set; } = new();
    }

}