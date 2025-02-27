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
using Domain.Entities.Media;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Words;
using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Storage;

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
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            // Ensure that the passed transaction is the current one (if you're tracking it)
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}
