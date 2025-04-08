using System;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.SpacedRepetition;
using Domain.Entities.Learning.Words;
using Domain.Entities.Media;
using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace Application.Interface;

public interface IApplicationDbContext
{
     DbSet<Question> Questions { get; set; }
    DbSet<QuestionOptionBase> QuestionOptions { get; set; }
    DbSet<UserProfile> UserProfiles { get; set; }
    DbSet<UserActivity> UserActivities { get; set; }
    DbSet<UserStats> UserStats { get; set; }
    DbSet<Course> Courses { get; set; }
    DbSet<Lesson> Lessons { get; set; }
    DbSet<LearningProgress> LearningProgresses { get; set; }
    DbSet<MultipleChoiceQuestionOption> MultipleChoiceQuestionOptions { get; set; }
    DbSet<MatchingQuestionOption> MatchingQuestionOptions { get; set; }
    DbSet<BuildSentenceQuestionOption> BuildSentenceQuestionOptions { get; set; }
    DbSet<PronunciationQuestionOption> PronunciationQuestionOptions { get; set; }
    DbSet<QuestionWord> QuestionWords { get; set; }
    DbSet<Configuration> Configurations { get; set; }
    DbSet<Option> Options { get; set; }
    DbSet<Word> Words { get; set; }
    DbSet<Media> Medias { get; set; }
    DbSet<Subscription> Subscriptions { get; set; }
    DbSet<SubscriptionType> SubscriptionTypes { get; set; }
    DbSet<SpacedRepetitionRecord> SpacedRepetitionRecords { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
}