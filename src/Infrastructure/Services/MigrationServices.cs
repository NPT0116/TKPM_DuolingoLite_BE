using System;
using Infrastructure.Data;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class MigrationServices: IHostedService
{
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationServices> _logger;
        private readonly IConfiguration _configuration;

        public MigrationServices(IServiceProvider serviceProvider, ILogger<MigrationServices> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying migrations.");
            }

            var seedResult = await SeedData.Initialize(scope.ServiceProvider); // Chạy seed
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string jsonResult = JsonConvert.SerializeObject(seedResult, settings);
            Console.WriteLine(jsonResult);
            try
            {
                var seedUser = scope.ServiceProvider.GetRequiredService<SeedUser>();
                var userSettings = _configuration.GetSection("Seed:Users").Get<UserSettings>();
                await seedUser.SeedUsersWithActivitiesAsync(userSettings.Password, userSettings.NumberOfUsers, userSettings.NumberOfDays);  // Seed 10 users with 7 days of activities
                _logger.LogInformation("Users and activities seeded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding users.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
