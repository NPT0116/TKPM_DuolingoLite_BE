using Application.Interface;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.Question;
using Domain.Entities.Question.QuestionOption;
using Domain.Entities.User;
using Domain.Entities.Course;
using Domain.Entities.Option;
using Domain.Entities.Word;
using Domain.Entities.Media;
using Domain.Entities.Subscription;
using Domain.Entities.Learning.LearningProgress;
using Domain.Entities.Learning.Question.QuestionOption;
using Domain.Entities.Learning.Question.Configuration;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOptionBase> QuestionOptions { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }
        public DbSet<MultipleChoiceQuestionOption> MultipleChoiceQuestionOptions { get; set; }
        public DbSet<MatchingQuestionOption> MatchingQuestionOptions { get; set; }
        public DbSet<BuildSentenceQuestionOption> BuildSentenceQuestionOptions { get; set; }
        public DbSet<PronunciationQuestionOption> PronunciationQuestionOptions { get; set; }
        public DbSet<QuestionWord> QuestionWords { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
    }
}
