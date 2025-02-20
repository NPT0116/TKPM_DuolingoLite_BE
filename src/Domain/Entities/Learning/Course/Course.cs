using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Course
{
    public class Course : Entity
    {
        public string  Name { get; private set; } = string.Empty;
        public int Level { get; private set; }
        public Course? NextCourse { get; private set; }
        private readonly List<Lesson> _lessons = new();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        private Course() { }

        private Course(string name, int level, Course? nextCourse)
        {
            Name = name;
            Level = level;
            NextCourse = nextCourse;
        }

        public static Result<Course> Create(string name, int level, Course? nextCourse)
        {
            if(string.IsNullOrEmpty(name))
            {
                return Result.Failure<Course>(CourseError.NameIsRequired());
            }

            if(level < 1)
            {
                return Result.Failure<Course>(CourseError.LevelMustBeGreaterThanZero());
            }
            return new Course(name, level, nextCourse);
        }

        public void SetNextCourse(Course nextCourse)
        {
            NextCourse = nextCourse;
        }

        public void AddLesson(Lesson lesson)
        {
            _lessons.Add(lesson);
        }

        public void RemoveLesson(Lesson lesson)
        {
            _lessons.Remove(lesson);
        }
        
        
    }
}