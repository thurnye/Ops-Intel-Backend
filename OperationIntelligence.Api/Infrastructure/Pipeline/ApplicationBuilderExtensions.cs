using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Api
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseAppPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OperationIntelligenceDbContext>();
                db.Database.EnsureCreated();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OperationIntelligence API v1");
                });
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<SanitizationMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
