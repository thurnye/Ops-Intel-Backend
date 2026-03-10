using FluentValidation;
using FluentValidation.AspNetCore;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddAppControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationResponseFilter>();
            });

            return services;
        }

        public static IServiceCollection AddAppValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            return services;
        }
    }
}
