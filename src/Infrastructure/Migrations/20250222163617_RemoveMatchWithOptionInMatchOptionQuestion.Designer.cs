﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250222163617_RemoveMatchWithOptionInMatchOptionQuestion")]
    partial class RemoveMatchWithOptionInMatchOptionQuestion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Learning.Courses.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("NextCourseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("NextCourseId")
                        .IsUnique();

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Domain.Entities.Learning.LearningProgresses.LearningProgress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<int>("LessonOrder")
                        .HasColumnType("integer");

                    b.Property<Guid?>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("userId");

                    b.ToTable("LearningProgresses");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Lessons.Lesson", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("XpEarned")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Lessons");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Configurations.Configuration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Audio")
                        .HasColumnType("boolean");

                    b.Property<bool>("EnglishText")
                        .HasColumnType("boolean");

                    b.Property<bool>("Image")
                        .HasColumnType("boolean");

                    b.Property<bool>("Instruction")
                        .HasColumnType("boolean");

                    b.Property<bool>("VietnameseText")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Options.Option", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AudioId")
                        .HasColumnType("uuid");

                    b.Property<string>("EnglishText")
                        .HasColumnType("text");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uuid");

                    b.Property<string>("VietnameseText")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AudioId");

                    b.HasIndex("ImageId");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AudioId")
                        .HasColumnType("uuid");

                    b.Property<string>("EnglishText")
                        .HasColumnType("text");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("uuid");

                    b.Property<string>("Instruction")
                        .HasColumnType("text");

                    b.Property<Guid?>("LessonId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("OptionConfigurationId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("QuestionConfigurationId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("VietnameseText")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AudioId");

                    b.HasIndex("ImageId");

                    b.HasIndex("LessonId");

                    b.HasIndex("OptionConfigurationId")
                        .IsUnique();

                    b.HasIndex("QuestionConfigurationId")
                        .IsUnique();

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("OptionId")
                        .HasColumnType("uuid");

                    b.Property<string>("OptionType")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("character varying(21)");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionOptions", (string)null);

                    b.HasDiscriminator<string>("OptionType").HasValue("QuestionOptionBase");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.QuestionWord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WordId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("WordId");

                    b.ToTable("QuestionWords");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Words.Word", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AudioId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("AudioId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Domain.Entities.Media.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MimeType")
                        .HasColumnType("integer");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Medias");
                });

            modelBuilder.Entity("Domain.Entities.Subscriptions.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ExpiredDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("SubscriptionTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionTypeId")
                        .IsUnique();

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("Domain.Entities.Subscriptions.SubscriptionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("DurationInMonth")
                        .HasColumnType("integer");

                    b.Property<long>("Price")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("SubscriptionTypes");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserActivity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("userId");

                    b.ToTable("UserActivities");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ProfileImageId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SubscriptionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProfileImageId");

                    b.HasIndex("SubscriptionId");

                    b.HasIndex("userId")
                        .IsUnique();

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserStats", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CurrentStreak")
                        .HasColumnType("integer");

                    b.Property<int>("ExperiencePoint")
                        .HasColumnType("integer");

                    b.Property<int>("Heart")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastActiveDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LongestStreak")
                        .HasColumnType("integer");

                    b.Property<Guid?>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("userId")
                        .IsUnique();

                    b.ToTable("UserStats");
                });

            modelBuilder.Entity("Infrastructure.Identity.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.BuildSentenceQuestionOption", b =>
                {
                    b.HasBaseType("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase");

                    b.Property<int>("Position")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("BuildSentence");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.MatchingQuestionOption", b =>
                {
                    b.HasBaseType("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase");

                    b.Property<int>("SourceType")
                        .HasColumnType("integer");

                    b.Property<int>("TargetType")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("Matching");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.MultipleChoiceQuestionOption", b =>
                {
                    b.HasBaseType("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("boolean");

                    b.HasDiscriminator().HasValue("MultipleChoice");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.PronunciationQuestionOption", b =>
                {
                    b.HasBaseType("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase");

                    b.HasDiscriminator().HasValue("Pronunciation");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Courses.Course", b =>
                {
                    b.HasOne("Domain.Entities.Learning.Courses.Course", "NextCourse")
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Learning.Courses.Course", "NextCourseId");

                    b.Navigation("NextCourse");
                });

            modelBuilder.Entity("Domain.Entities.Learning.LearningProgresses.LearningProgress", b =>
                {
                    b.HasOne("Domain.Entities.Learning.Courses.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("userId");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Lessons.Lesson", b =>
                {
                    b.HasOne("Domain.Entities.Learning.Courses.Course", null)
                        .WithMany("Lessons")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Options.Option", b =>
                {
                    b.HasOne("Domain.Entities.Media.Media", "Audio")
                        .WithMany()
                        .HasForeignKey("AudioId");

                    b.HasOne("Domain.Entities.Media.Media", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.Navigation("Audio");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Question", b =>
                {
                    b.HasOne("Domain.Entities.Media.Media", "Audio")
                        .WithMany()
                        .HasForeignKey("AudioId");

                    b.HasOne("Domain.Entities.Media.Media", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("Domain.Entities.Learning.Lessons.Lesson", null)
                        .WithMany("Questions")
                        .HasForeignKey("LessonId");

                    b.HasOne("Domain.Entities.Learning.Questions.Configurations.Configuration", "OptionConfiguration")
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Learning.Questions.Question", "OptionConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Learning.Questions.Configurations.Configuration", "QuestionConfiguration")
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Learning.Questions.Question", "QuestionConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Audio");

                    b.Navigation("Image");

                    b.Navigation("OptionConfiguration");

                    b.Navigation("QuestionConfiguration");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.QuestionOptionBase", b =>
                {
                    b.HasOne("Domain.Entities.Learning.Questions.Options.Option", "Option")
                        .WithMany()
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Learning.Questions.Question", "Question")
                        .WithMany("Options")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.QuestionOptions.QuestionWord", b =>
                {
                    b.HasOne("Domain.Entities.Learning.Questions.Question", "Question")
                        .WithMany("Words")
                        .HasForeignKey("QuestionId");

                    b.HasOne("Domain.Entities.Learning.Words.Word", "Word")
                        .WithMany()
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("Word");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Words.Word", b =>
                {
                    b.HasOne("Domain.Entities.Media.Media", "Audio")
                        .WithMany()
                        .HasForeignKey("AudioId");

                    b.Navigation("Audio");
                });

            modelBuilder.Entity("Domain.Entities.Subscriptions.Subscription", b =>
                {
                    b.HasOne("Domain.Entities.Subscriptions.SubscriptionType", "SubscriptionType")
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Subscriptions.Subscription", "SubscriptionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubscriptionType");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserActivity", b =>
                {
                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("userId");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserProfile", b =>
                {
                    b.HasOne("Domain.Entities.Media.Media", "ProfileImage")
                        .WithMany()
                        .HasForeignKey("ProfileImageId");

                    b.HasOne("Domain.Entities.Subscriptions.Subscription", "Subscription")
                        .WithMany()
                        .HasForeignKey("SubscriptionId");

                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Users.UserProfile", "userId");

                    b.Navigation("ProfileImage");

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("Domain.Entities.Users.UserStats", b =>
                {
                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("Domain.Entities.Users.UserStats", "userId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.Learning.Courses.Course", b =>
                {
                    b.Navigation("Lessons");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Lessons.Lesson", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Domain.Entities.Learning.Questions.Question", b =>
                {
                    b.Navigation("Options");

                    b.Navigation("Words");
                });
#pragma warning restore 612, 618
        }
    }
}
