using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Commands.Register;
using Bogus;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Seed
{
    public class SeedUser
    {
        private readonly UserRegisterCommandHandler _userRegisterCommandHandler;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public SeedUser(UserRegisterCommandHandler userRegisterCommandHandler, IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRegisterCommandHandler = userRegisterCommandHandler;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task SeedUsersWithActivitiesAsync(string password, int numberOfUsers, int numberOfDays)
        {
            var totalUsers = await _userRepository.GetTotalUsersCount();
            if(totalUsers > 0) return;

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
    }
}