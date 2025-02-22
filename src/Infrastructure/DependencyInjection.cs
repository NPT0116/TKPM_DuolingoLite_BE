using System.Text;
using Application.Interface;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<JwtService>();
        services.AddHttpContextAccessor();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHostedService<MigrationServices>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<ILearningProgressRepository, LearningProgressRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<IQuestionWordRepository,QuestionWordRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        return services;
    }
}
