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
    using SharedKernel;
    using Domain.Entities.Learning.Questions.Enums;
    using Domain.Entities.Learning.Questions.Configurations;
    using Domain.Entities.Media;
    using Domain.Entities.Media.Enums;
    using Domain.Entities.Learning.Questions.Options;
    using Domain.Entities.Learning.Questions.QuestionOptions;
    using Domain.Entities.Learning.Words;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;
    using Infrastructure.Services;
    using Amazon.Runtime.Internal.Auth;
    using Application.Common.Interface;
    using Application.Interface;

    public static class SeedData
    {
        private static async Task<Result> SeedCourse(IApplicationDbContext context, string filePath)
        {
            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"❌ File not found: {Path.GetFullPath(filePath)}");
            }

            // Đọc file JSON
            var json = File.ReadAllText(filePath);

                var courseModel = JsonConvert.DeserializeObject<CourseJsonModel>(json);

                if(courseModel == null)
                {
                    return Result.Failure(Error.NullValue);
                }

                var course = Course.Create(
                    courseModel.Name,
                    courseModel.Level,
                    null
                );

                if(course.IsFailure)
                {
                    return Result.Failure(course.Error);
                }

                var lessons = courseModel.Lessons;
                foreach(var lesson in lessons)
                {
                    var newLesson = Lesson.Create(
                        lesson.Title,
                        lesson.XpEarned,
                        lesson.Order
                    );

                    if(newLesson.IsFailure)
                    {
                        return Result.Failure(newLesson.Error);
                    }
                    
                    var questions = lesson.Questions;
                    foreach(var question in questions)
                    {
                        
                        var audio = question.Audio == null ? null :Media.Create("audio", MediaType.Audio, 10000, question.Audio, question.Audio);
                        if(audio != null && audio.IsFailure)
                        {
                            return Result.Failure(audio.Error);
                        }

                        var image = question.Image == null ? null : Media.Create("image", MediaType.Image, 100000, question.Image, question.Image);
                        if(image != null && image.IsFailure)
                        {
                            return Result.Failure(image.Error);
                        }

                        var questionConfiguration = Configuration.Create(
                            question.QuestionConfiguration.Audio,
                            question.QuestionConfiguration.EnglishText,
                            question.QuestionConfiguration.VietnameseText,
                            question.QuestionConfiguration.Instruction,
                            question.QuestionConfiguration.Image
                        );

                        if(questionConfiguration.IsFailure)
                        {
                            return Result.Failure(questionConfiguration.Error);
                        }

                        var optionConfiguration = Configuration.Create(
                            question.OptionConfiguration.Audio,
                            question.OptionConfiguration.EnglishText,
                            question.OptionConfiguration.VietnameseText,
                            question.OptionConfiguration.Instruction,
                            question.OptionConfiguration.Image
                        );

                        if(optionConfiguration.IsFailure)
                        {
                            return Result.Failure(optionConfiguration.Error);
                        }
                        
                        var newQuestion = Question.Create(
                            question.Instruction,
                            question.VietnameseText,
                            audio != null ? audio.Value : null,
                            question.EnglishText,
                            image != null ? image.Value : null,
                            Enum.Parse<QuestionType>(question.Type),
                            questionConfiguration.Value,
                            optionConfiguration.Value,
                            question.Order
                        );

                        if(newQuestion.IsFailure)
                        {
                            return Result.Failure(newQuestion.Error);
                        }

                        var options = question.Options;
                        foreach(var option in options)
                        {
                            Option newOption;
                            var existingOption = context.Options.FirstOrDefault(
                                o => o.VietnameseText == option.VietnameseText
                                && o.EnglishText == option.EnglishText);
                            if(existingOption != null)
                            {
                                newOption = existingOption;
                            }
                            else
                            {
                                var optionImage = option.Image == null ? null : Media.Create("optionImage", MediaType.Image, 100000, option.Image, option.Image);
                                if(optionImage != null && optionImage.IsFailure)
                                {
                                    return Result.Failure(optionImage.Error);
                                }

                                var optionAudio = option.Audio == null ? null : Media.Create("optionAudio", MediaType.Audio, 10000, option.Audio, option.Audio);
                                if(optionAudio != null && optionAudio.IsFailure)
                                {
                                    return Result.Failure(optionAudio.Error);
                                }

                                var newOptionResult = Option.Create(
                                    option.VietnameseText,
                                    optionImage != null ? optionImage.Value : null,
                                    optionAudio != null ? optionAudio.Value : null,
                                    option.EnglishText
                                );

                                if(newOptionResult.IsFailure)
                                {
                                    return Result.Failure(newOptionResult.Error);
                                }

                                newOption = newOptionResult.Value;
                                context.Options.Add(newOption);
                                await context.SaveChangesAsync();
                            }

                            if(option == null)
                            {
                                Console.WriteLine("Option is null");
                            }

                            if(newQuestion.Value == null)
                            {
                                Console.WriteLine("New question is null");
                            }

                            if(newOption == null)
                            {
                                Console.WriteLine("New option is null");
                            }

                            if (question.Type == "MultipleChoice")
                            {
                                var _settings = new JsonSerializerSettings
                                {
                                    Formatting = Formatting.Indented, // Format dễ đọc hơn
                                    NullValueHandling = NullValueHandling.Include // Không bỏ qua giá trị null
                                };

                                Console.WriteLine(JsonConvert.SerializeObject(option, _settings)); // ✅ Dùng JsonConvert

                                Console.WriteLine(JsonConvert.SerializeObject(option as MatchingOptionJsonModel, _settings)); // ✅ Dùng JsonConvert
                            }

                            QuestionOptionBase newQuestionOption = question.Type switch
                            {
                                "MultipleChoice" => MultipleChoiceQuestionOption.Create(newQuestion.Value, newOption, (option as MultipleChoiceOptionJsonModel)?.IsCorrect ?? false, 1).Value,

                                "Matching" => MatchingQuestionOption.Create(newQuestion.Value, newOption,  
                                                                            Enum.Parse<MatchingQuestionOptionType>((option as MatchingOptionJsonModel).SourceType), 
                                                                            Enum.Parse<MatchingQuestionOptionType>((option as MatchingOptionJsonModel).TargetType), 
                                                                            1).Value,

                                "BuildSentence" => BuildSentenceQuestionOption.Create(newQuestion.Value, newOption, 1, (option as BuildSentenceOptionJsonModel)?.Order ?? -1).Value,

                                "Pronunication" => PronunciationQuestionOption.Create(newQuestion.Value, newOption, 1).Value,

                                _ => throw new InvalidOperationException("Invalid question type")
                            };
                            newQuestion.Value.AddOption(newQuestionOption); 

                        }
                            var words = question.Words;
                            if(words != null)
                            {
                                var order = 1;
                                Console.WriteLine("Words: " + words.Count());
                                foreach(var word in words)
                                {

                                    var existingWord = context.Words.FirstOrDefault(w => w.Content == word.Content);

                                    if (existingWord == null)
                                    {
                                        // Nếu Word chưa tồn tại, tạo mới và thêm vào DbContext
                                        string wordAudio = word.Audio;
                                        
                                        var createdWord = Word.Create(word.Content,
                                            wordAudio == string.Empty 
                                                ? Media.Create("wordAudio", MediaType.Audio, 10000, word.Audio, "wordAudio").Value 
                                                : Media.Create("wordAudio", MediaType.Audio, 10000, wordAudio, "wordAudio").Value);

                                        if (createdWord.IsFailure)
                                        {
                                            return Result.Failure(createdWord.Error);
                                        }

                                        existingWord = createdWord.Value;
                                        context.Words.Add(existingWord);
                                        await context.SaveChangesAsync();
                                    }

                                    var newQuestionWord = QuestionWord.Create(
                                        existingWord,
                                        newQuestion.Value,
                                        order++
                                    );

                                    if(newQuestionWord.IsFailure)
                                    {
                                        return Result.Failure(newQuestionWord.Error);
                                    }

                                    newQuestion.Value.AddWord(newQuestionWord.Value);
                                }
                            }

                        newLesson.Value.AddQuestion(newQuestion.Value);
                    }

                    course.Value.AddLesson(newLesson.Value);
                }

                context.Courses.Add(course.Value);
                await context.SaveChangesAsync();

                return Result.Success();
        }
        public static async Task<Result> Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IApplicationDbContext>();
            using var transaction = await context.BeginTransactionAsync();

            if(context.Courses.Any()) return Result.Success();

            var seedCourses = await SeedCourses(context, serviceProvider);
            if(seedCourses.IsFailure)
            {
                await context.RollbackTransactionAsync(transaction);
            }
            await context.SaveChangesAsync();
            await context.CommitTransactionAsync(transaction);
            return Result.Success();
        }

        private static async Task<Result> SeedCourses(IApplicationDbContext context, IServiceProvider serviceProvider)
        {
            var filePaths = new List<string>()
            {
                Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Persistence", "Seed", "course.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Persistence", "Seed", "course2.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Persistence", "Seed", "course1.json"),
            };

            foreach(var filePath in filePaths)
            {
                Console.WriteLine("FILE PATH-------------------------------------:" + filePath);
                var seedCourse = await SeedCourse(context, filePath);
                if(seedCourse.IsFailure) return Result.Failure(seedCourse.Error);
            }

            return Result.Success();
        }
    }

}