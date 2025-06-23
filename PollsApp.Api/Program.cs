using PollsApp.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// 1) Registrar serviços
builder.Services
    .AddDatabase(builder.Configuration)
    .AddMigrations(builder.Configuration)
    .AddRedis(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationServices()
    .AddSwaggerWithControllers();

var app = builder.Build();

// 2) Aplicar migrations
app.RunMigrations();

// 3) Configurar pipeline
if (app.Environment.IsDevelopment())
    app.UseDevSwagger();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
