using FluentMigrator.Runner;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PollsApp.Api.Middlewares;
using PollsApp.Application.Jobs;

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

        public static void AddMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLogContextMiddleware>();
        }

        public static void LoadRecurrentJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<PollsJobs>(
                "expired-polls",
                job => job.CloseExpiredPollsAsync(),
                Cron.Minutely
            );
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
