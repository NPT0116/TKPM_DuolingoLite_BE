using System;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.Configurations;

public static class ConfigurationError
{
    public static Error ConfigurationNotFound(Guid configurationId) => Error.NotFound("Configuration.ConfigurationNotfound",$"Configuration with id {configurationId} is not found.");
}
