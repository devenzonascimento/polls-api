using Hangfire;
using PollsApp.Api.Extensions;
using Serilog;

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

app.UseHangfireDashboard("/hangfire");
app.LoadRecurrentJobs();
app.UseSerilogRequestLogging();
app.AddMiddlewares();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
