using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Courses
{
    public class CourseError
    {
        public static Error NameIsRequired() => Error.Validation("Course.NameIsRequired", "Name is required");
        public static Error LevelMustBeGreaterThanZero() => Error.Validation("Course.LevelMustBeGreaterThanZero", "Level must be greater than zero");
        public static Error CourseNotFound(Guid id) => Error.NotFound("Course.CourseNotFound", $"Course with id {id} not found");
    }
}