using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;

namespace OperationIntelligence.Api
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OperationIntelligence API",
                    Version = "v1"
                });

                options.TagActionsBy(api =>
                {
                    if (api.ActionDescriptor is not ControllerActionDescriptor cad)
                        return new[] { "default" };

                    var ns = cad.ControllerTypeInfo.Namespace ?? string.Empty;
                    var marker = ns.Contains(".Controllers.", StringComparison.OrdinalIgnoreCase)
                        ? ".Controllers."
                        : ".Controller.";
                    var index = ns.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

                    if (index < 0)
                        return new[] { cad.ControllerName.ToLowerInvariant() };

                    var group = ns[(index + marker.Length)..]
                        .Split('.', StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault();

                    var top = string.IsNullOrWhiteSpace(group) ? "default" : group.ToLowerInvariant();
                    var controller = cad.ControllerName.ToLowerInvariant();

                    return new[] { $"{top}/{controller}" };
                });
            });

            return services;
        }
    }
}
