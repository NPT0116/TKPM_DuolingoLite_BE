using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Commands.Register;
using Application.Interface;
using Application.Interface;
using Bogus;
using Domain.Entities.Users;
using Domain.Repositories;
using Google.Api;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using SharedKernel;

namespace Infrastructure.Persistence.Seed
{
    public class SeedUser
    {
        private readonly UserRegisterCommandHandler _userRegisterCommandHandler;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        
        public SeedUser(
            UserRegisterCommandHandler userRegisterCommandHandler, 
            IUserRepository userRepository, 
            ApplicationDbContext context,
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            IIdentityService identityService)
        {
            _userRegisterCommandHandler = userRegisterCommandHandler;
            _userRepository = userRepository;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _identityService = identityService;
        }

        public async Task SeedUsersWithActivitiesAsync(string password, int numberOfUsers, int numberOfDays)
        {
            var totalUsers = await _userRepository.GetTotalUsersCount();
            if(totalUsers > 0) return;
            // 1. Lấy toàn bộ tên role đã có trong DB trước, giảm số lần gọi SQL
            var existingRoles = _roleManager.Roles
                                            .Select(r => r.Name)
                                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 2. Duyệt qua mọi giá trị enum và tạo nếu còn thiếu
            foreach (var roleName in Enum.GetNames<Domain.Entities.Users.Role>())   // hoặc Enum.GetValues<Role>()
            {
                if (!existingRoles.Contains(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
            await _context.SaveChangesAsync();
            
            var seedAdmin = await SeedAdminUser();
            


            var faker = new Faker<UserRegisterCommand>()
            .CustomInstantiator(f => new UserRegisterCommand(new UserRegisterDto(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                f.Internet.UserName(),
                password
            ), null));

            

            for (int i = 0; i < numberOfUsers; i++)
            {
                var userRegisterCommand = faker.Generate();
                var result = await _userRegisterCommandHandler.Handle(userRegisterCommand, CancellationToken.None);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"User {userRegisterCommand.UserRegisterDto.Email} created with ID {result.Value}");
                    await SeedDailyActivities(result.Value, numberOfDays);
                }
                else
                {
                    Console.WriteLine($"Failed to create user {userRegisterCommand.UserRegisterDto.Email}: {result.Error}");
                }
            }

            await _context.SaveChangesAsync();

            var stats = await _context.UserStats.ToListAsync();
            var random = new Random();
            foreach (var stat in stats)
            {
                int xp = random.Next(0, 51) * 10; // random số từ 0 đến 50, rồi nhân 10
                stat.EarnExperience(xp);
            }

            await _context.SaveChangesAsync();

        }

        private async Task SeedDailyActivities(Guid userId, int numberOfDays)
        {
            var startDate = DateTime.UtcNow.AddDays(-numberOfDays - 1).Date;

            var faker = new Faker<UserActivity>()
                .CustomInstantiator(f => 
                {
                    var date = startDate.AddDays(f.IndexFaker + 1);
                    Console.WriteLine($"Date: {f.IndexFaker}");
                    return UserActivity.Create(
                        userId: userId,
                        date: date,
                        isActive: f.Random.Bool()
                    ).Value;
                });

            // Generate activities with continuous unique dates
            var activities = faker.Generate(numberOfDays);

            foreach (var activity in activities)
            {
                await _userRepository.CreateUserActivity(activity);
                Console.WriteLine($"UserId: {activity.UserId}, Date: {activity.Date}, IsActive: {activity.IsActive}");
            }

        }

        private async Task<Result> SeedAdminUser()
        {
            var admin = new ApplicationUser()
            {
                FirstName = "Nguyen",
                LastName = "Hong Quan"
            };

            var firstName = "Nguyen";
            var lastName = "Hong Quan";
            var email = "nquan003@gmail.com";
            var username = "admin";
            var password = "123456";

            var result = await _identityService.CreateUserAsync(
                firstName,
                lastName,
                email,
                username,
                password
            );

            if(result.Result.IsFailure) return Result.Failure(result.Result.Error);

            var userId = result.UserId;

            var userActivity = UserActivity.Create(userId, DateTime.UtcNow, true);
            if (userActivity.IsFailure)
            {
                return Result.Failure(userActivity.Error);
            }

            var userStats = UserStats.Create(userId);
            if (userStats.IsFailure)
            {
                throw new ApplicationErrorException(userStats.Error);
            }

            var userProfile = Domain.Entities.Users.UserProfile.Create(
                userId,
                email,
                username,
                firstName,
                lastName,
                null,
                null);

            if (userProfile.IsFailure)
            {
                // Optionally, if an avatar was uploaded, you might consider deleting it here.
                return Result.Failure(userProfile.Error);
            }

            // await _userRepository.CreateUserActivity(userActivity.Value);
            await _userRepository.CreateUserStats(userStats.Value);
            await _userRepository.CreateUserProfile(userProfile.Value);

            await _context.SaveChangesAsync();
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(adminUser != null)
                await _userManager.AddToRoleAsync(adminUser, Domain.Entities.Users.Role.Admin.ToString());

            return Result.Success();
        }
    }
}