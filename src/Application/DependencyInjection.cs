﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly  = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));
            
            services.AddValidatorsFromAssembly(assembly);
            return services;
        }
    }
}
