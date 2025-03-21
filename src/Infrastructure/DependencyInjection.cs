﻿using System.Text;
using Amazon.S3;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.User.Commands.Register;
using Application.Interface;
using Application.Interfaces;
using Domain.Repositories;
using Domain.Service;
using Google.Cloud.TextToSpeech.V1;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services;
using Infrastructure.Services.Payment;
using Infrastructure.Services.Settings;
using Infrastructure.Token;
using Infrastructure.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using Quartz;
using StackExchange.Redis;
using VNPAY.NET;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine(connectionString);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6; // Set your desired minimum length
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (string.IsNullOrEmpty(jwtSettings?.Secret))
        {
            throw new ArgumentNullException("JwtSettings:Secret", 
                "JWT Secret key is not configured in appsettings.json");
        }

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret ?? 
                        throw new InvalidOperationException("JWT Secret cannot be null")))
            };
        });

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            BackgroundSettings backgroundSettings = configuration
                .GetSection("BackgroundJobs")
                .Get<BackgroundSettings>();


            var heartSyncJobKey = JobKey.Create(nameof(HeartSyncBackgroundService));
            options
                .AddJob<HeartSyncBackgroundService>(heartSyncJobKey)
                .AddTrigger(trigger => trigger
                    .ForJob(heartSyncJobKey)
                    .WithCronSchedule(backgroundSettings.HeartSyncInterval));

            var refillHeartSyncJobKey = JobKey.Create(nameof(HeartRecoveryBackgroundService));
            options
                .AddJob<HeartRecoveryBackgroundService>(refillHeartSyncJobKey)
                .AddTrigger(trigger => trigger
                    .ForJob(refillHeartSyncJobKey)
                    .WithCronSchedule(backgroundSettings.RefillHeartCheckInterval));
        });


        services.AddQuartzHostedService();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<JwtService>();
        services.AddHttpContextAccessor();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<SeedUser>();
        services.AddScoped<UserRegisterCommandHandler>();

        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ILearningProgressRepository, LearningProgressRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<IQuestionWordRepository,QuestionWordRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISpeechToTextService , SpeechToTextService>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IMomoService, MomoService>();
        services.AddScoped<IStreakService, StreakService>();
        services.AddScoped<IDateTimeProvider    , DateTimeProvider>();
        services.AddSingleton<IVnpay, Vnpay>();
        // services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        // services.AddAWSService<IAmazonS3>();
        services.Configure<BackgroundSettings>(configuration.GetSection("BackgroundJobs"));
        services.Configure<AwsSettings>(configuration.GetSection("AWS"));
        services.AddScoped<ITokenService, TokenService>();
        
        // Optionally, you can register the settings as a singleton:
        var awsSettings = new AwsSettings();
        configuration.GetSection("AWS").Bind(awsSettings);
        services.AddSingleton(awsSettings);

        // Register your AWS services using these settings (example for AmazonS3Client)
        var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region);
        var s3Client = new AmazonS3Client(awsSettings.AccessKey, awsSettings.SecretKey, regionEndpoint);
        services.AddSingleton<IAmazonS3>(s3Client);
        services.AddScoped<IMediaStorageService, AwsS3StorageService>();

        // services.Configure<AwsSettings>(configuration.GetSection("AWS"));
        var mediaSettings = configuration.GetSection("MediaSettings").Get<MediaSettings>();
        services.AddSingleton(mediaSettings);
        
        
        services.AddScoped<ITextToSpeechService, GoogleCloudTextToSpeechService>();
        services.AddHttpClient<IDictionaryService, DictionaryService>(client =>
        {
            client.BaseAddress = new Uri("https://api.dictionaryapi.dev/");
            // Optionally configure default headers, timeouts, etc.
        });


        var redisConnectionString = configuration.GetValue<string>("ConnectionStrings:Redis");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            // Retrieve the Redis connection string from configuration.
            var configuration = sp.GetRequiredService<IConfiguration>();
            
            // Optionally, you can use ConfigurationOptions to configure advanced settings.
            var options = new StackExchange.Redis.ConfigurationOptions
            {
                AbortOnConnectFail = true,
            };
            options.EndPoints.Add(redisConnectionString);

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            // options.InstanceName = "SampleInstance:";
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
            {
                AbortOnConnectFail = true,
                EndPoints = { options.Configuration }
            };
        });

        var assembly  = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));

        return services;
    }
}
