using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PollsApp.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void RunMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        public static WebApplication UseDevSwagger(this WebApplication app)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
