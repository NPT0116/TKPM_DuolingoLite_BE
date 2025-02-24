using System;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConfigurationRepository : IConfigurationRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public ConfigurationRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<Configuration?> GetConfigureById(Guid configureId)
    {
        return await _applicationDbContext.Configurations.FirstOrDefaultAsync(c => c.Id ==  configureId);
    }
}
