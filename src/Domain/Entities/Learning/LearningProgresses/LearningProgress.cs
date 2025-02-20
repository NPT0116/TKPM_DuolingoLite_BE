using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Courses;
using Domain.Entities.User;
using SharedKernel;

namespace Domain.Entities.Learning.LearningProgresses
{
    public class LearningProgress : Entity
    {
        public int LessonOrder { get; private set; }
        public bool IsCompleted { get; private set; }
        public Course Course { get; private set; }
        
        private LearningProgress() 
        {
        }

        private LearningProgress(
            Course course
        )
        {
            Course = course;
            LessonOrder = 1;
            IsCompleted = false;
        }

        public static Result<LearningProgress> Create(
            Course course
        )
        {
            return Result.Success(new LearningProgress(course));
        }

    }
}