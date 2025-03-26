using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.QuestionOptions.Factory;
using Domain.Entities.Learning.Questions.QuestionOptions.Validator;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddScoped<IQuestionOptionFactory, QuestionOptionFactory>();
            services.AddScoped<IQuestionOptionValidator, QuestionOptionValidator>();
            services.AddScoped<IQuestionFactory, QuestionFactory>();
            return services;
        }
    }
}