using System;
using Domain.Entities.Learning.Questions.Configurations;

namespace Domain.Repositories;

public interface IConfigurationRepository
{
    public Task<Configuration?> GetConfigureById(Guid configureId);
}
