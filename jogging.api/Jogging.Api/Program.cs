using Jogging.Api.Configuration;
using Jogging.Infrastructure.Services;
using Jogging.Persistence.Context;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Jogging.Api;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        // ────────────────────────────────────────────────────────
        // Serilog configuratie (leest settings uit appsettings.json)
        // ────────────────────────────────────────────────────────
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        try
        {
            Log.Information("Web host wordt gestart...");

            // ────────────────────────────────────────────────────────
            // 1) Database-connectie met MySQL
            // ────────────────────────────────────────────────────────
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<JoggingContext>(opts =>
                opts.UseMySql(connStr, ServerVersion.AutoDetect(connStr))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            // ────────────────────────────────────────────────────────
            // 2) BackupService registreren
            // ────────────────────────────────────────────────────────
            builder.Services.AddScoped<IBackupService, BackupService>();

            // ────────────────────────────────────────────────────────
            // 3) MultiSafepay, SMTP, domain- en helper-services
            // ────────────────────────────────────────────────────────
            builder.Services.AddMultiSafepay(builder.Configuration);
            builder.Services.AddSmtpEmailClient(builder.Configuration);
            builder.Services.AddInterfaces();
            builder.Services.AddDomainManagerServices();
            builder.Services.AddHelperServices();

            // ────────────────────────────────────────────────────────
            // 4) In-memory cache
            // ────────────────────────────────────────────────────────
            builder.Services.AddMemoryCache();

            // ────────────────────────────────────────────────────────
            // 5) AutoMapper
            // ────────────────────────────────────────────────────────
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            // ────────────────────────────────────────────────────────
            // 6) Lowercase URLs
            // ────────────────────────────────────────────────────────
            builder.Services.Configure<RouteOptions>(opts =>
                opts.LowercaseUrls = true
            );

            // ────────────────────────────────────────────────────────
            // 7) Controllers en Swagger
            // ────────────────────────────────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers()
                   .AddApplicationPart(typeof(Jogging.Rest.Controllers.BackupController).Assembly)
                   .AddJsonOptions(opts =>
                   {
                       opts.JsonSerializerOptions.ReferenceHandler =
                           System.Text.Json.Serialization.ReferenceHandler.Preserve;
                   });
            builder.Services.AddSwaggerGen(c =>
            {
                // Configureer de Swagger-documentatie voor versie "v1"
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Jogging API",         // Titel van de API
                    Version = "v1",                // Versienummer
                    Description = configuration["Swagger:Description"]  // Beschrijving uit configuratie
                });

                // Definieer JWT-authenticatie voor Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",  // Uitleg over JWT-gebruik
                    Name = "Authorization",        // Naam van de header
                    In = ParameterLocation.Header, // Locatie van de token (in header)
                    Type = SecuritySchemeType.Http, // Type authenticatie (HTTP)
                    Scheme = "Bearer"              // Schema (Bearer token)
                });

                // Voeg een security-vereiste toe voor JWT
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,  // Verwijzing naar het security-schema
                                Id = "Bearer"                       // ID van het schema (moet overeenkomen met AddSecurityDefinition)
                            }
                        },
                        Array.Empty<string>()  // Geen extra scopes nodig
                    }
                });
            });


            // ────────────────────────────────────────────────────────
            // 8) CORS (origineel uit appsettings.json -> "AllowedOrigins")
            // ────────────────────────────────────────────────────────
            var origins = builder.Configuration
                                 .GetSection("AllowedOrigins")
                                 .Get<string[]>();
            builder.Services.AddCors(opt =>
                opt.AddPolicy("AllowSpecificOrigin", policy =>
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                )
            );

            // ────────────────────────────────────────────────────────
            // 9) Serilog als Logging-provider
            // ────────────────────────────────────────────────────────
            builder.Services.AddLogging(lb => lb.AddSerilog(dispose: true));

            // ────────────────────────────────────────────────────────
            // 10) Rate limiting en Helmet
            // ────────────────────────────────────────────────────────
            builder.Services.AddRateLimiter(RateLimiterConfigurator.ConfigureRateLimiter);
            builder.Services.AddXFrameSupress();

            // ────────────────────────────────────────────────────────
            // 11) Authenticatie
            // ────────────────────────────────────────────────────────
            builder.Services.AddCustomAuthentication(builder.Configuration);
            builder.Services.AddSingleton<Jogging.Domain.Configuration.EmailConfiguration>();
            builder.Services.AddTransient<System.Net.Mail.SmtpClient>();

            // ────────────────────────────────────────────────────────
            // Applicatie bouwen en starten
            // ────────────────────────────────────────────────────────
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "jogging-api v1")
            );

            app.UseCors("AllowSpecificOrigin");
            app.UseHelmetHeaders();
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host onverwacht beëindigd");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
