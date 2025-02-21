using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure.Persistence.Seed.JsonModels;

    public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (!context.Courses.Any())
        {
            var json = File.ReadAllText("Data/course.json");
            var courseModel = JsonSerializer.Deserialize<CourseJsonModel>(json);

            // if (courseModel != null)
            // {
            //     var course = new Course(courseModel.Name, courseModel.Level, null);

            //     foreach (var lessonModel in courseModel.Lessons)
            //     {
            //         var lesson = new Lesson(lessonModel.Title, lessonModel.XpEarned, lessonModel.Order);
                    
            //         foreach (var questionModel in lessonModel.Questions)
            //         {
            //             var question = new Question(
            //                 questionModel.Instruction,
            //                 questionModel.VietnameseText,
            //                 null,
            //                 questionModel.EnglishText,
            //                 null,
            //                 Enum.Parse<QuestionType>(questionModel.Type),
            //                 new Configuration(),
            //                 new Configuration(),
            //                 questionModel.Order
            //             );

            //             foreach (var optionModel in questionModel.Options)
            //             {
            //                 if (question.Type == QuestionType.MultipleChoice)
            //                     question.AddOption(new MultipleChoiceQuestionOption(question, new Option(optionModel.Text, null, null, null), optionModel.IsCorrect, 1));

            //                 if (question.Type == QuestionType.BuildSentence)
            //                     question.AddOption(new BuildSentenceQuestionOption(question, new Option(optionModel.Text, null, null, null), optionModel.Order, 1));

            //                 if (question.Type == QuestionType.Matching)
            //                     question.AddOption(new MatchingQuestionOption(question, new Option(optionModel.Text, null, null, null), new Option(optionModel.MatchWith, null, null, null), MatchingQuestionOptionType.EnglishText, MatchingQuestionOptionType.VietnameseText, 1));

            //                 if (question.Type == QuestionType.Pronunciation)
            //                     question.AddOption(new PronunciationQuestionOption(question, new Option(optionModel.Text, null, null, null), 1));
            //             }

            //             lesson._questions.Add(question);
            //         }
                    
            //         course.AddLesson(lesson);
            //     }

            //     context.Courses.Add(course);
            //     context.SaveChanges();
            // }
        }
    }
}

}