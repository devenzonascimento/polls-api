using Hangfire;
using PollsApp.Api.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration)
);

// 1) Registrar serviços
builder.Services
    .AddDatabase(builder.Configuration)
    .AddMigrations(builder.Configuration)
    .AddRedis(builder.Configuration)
    .AddHangfire(builder.Configuration)
    .AddOpenSearch(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationServices()
    .AddSwaggerWithControllers();

var app = builder.Build();

// 2) Aplicar migrations
app.RunMigrations();

// 3) Configurar pipeline
if (app.Environment.IsDevelopment())
    app.UseDevSwagger();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        var status = httpContext.Response.StatusCode;

        // Se houve exception ou 5xx -> Error
        if (ex != null || status >= 500)
            return LogEventLevel.Error;

        // Se 4xx -> Error em vez de Warning/Information
        if (status >= 400)
            return LogEventLevel.Error;

        // Caso contrário -> Info
        return LogEventLevel.Information;
    };
});

app.UseHangfireDashboard("/hangfire");
app.LoadRecurrentJobs();
app.AddMiddlewares();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
