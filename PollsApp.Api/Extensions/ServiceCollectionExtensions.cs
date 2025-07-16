using System.Data;
using System.Text;
using FluentMigrator.Runner;
using Hangfire;
using Hangfire.Redis;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PollsApp.Application.Jobs;
using PollsApp.Application.Services;
using PollsApp.Application.Services.Interfaces;
using PollsApp.Infrastructure.Data.Repositories;
using PollsApp.Infrastructure.Data.Repositories.Interfaces;
using PollsApp.Infrastructure.Data.Search;
using PollsApp.Infrastructure.Events;
using PollsApp.Infrastructure.Events.Interfaces;
using StackExchange.Redis;

namespace PollsApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var readOnly = config.GetConnectionString("PostgreSqlReadOnly");
            var readWrite = config.GetConnectionString("PostgreSqlReadWrite");

            services.AddScoped<IDbConnection>(sp =>
                new NpgsqlConnection(config.GetConnectionString("PostgreSql"))
            );

            return services;
        }

        public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration config)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(config.GetConnectionString("PostgreSql"))
                    .ScanIn(typeof(PollsApp.Infrastructure.Data.Migrations.CreateUsersTable).Assembly)
                        .For.Migrations()
                )
                .AddLogging(lb => lb.AddFluentMigratorConsole());
            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
        {
            var redisConn = config.GetConnectionString("Redis") ?? string.Empty;
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConn)
            );
            return services;
        }

        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
        {
            var redisConn = config.GetConnectionString("Redis");

            services.AddHangfireServer();

            services.AddHangfire(cfg =>
            {
                cfg.UseRedisStorage(redisConn, new RedisStorageOptions
                {
                    Prefix = "hangfire:",
                    InvisibilityTimeout = TimeSpan.FromMinutes(5),
                });
            });

            return services;
        }

        public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration config)
        {
            var openSearchConn = config.GetConnectionString("OpenSearch");

            OpenSearchConnectionSingleton.SetConnectionString(openSearchConn);

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var key = config["Jwt:Key"];
            var iss = config["Jwt:Issuer"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = iss,
                        ValidAudience = iss,
                        IssuerSigningKey = signingKey,
                    };
                });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(AuthService).Assembly);
            });

            services.AddScoped<PollsJobs>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPollSearchService, PollSearchService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IPollCommentRepository, PollCommentRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            return services;
        }

        public static IServiceCollection AddSwaggerWithControllers(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
