using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RO.DevTest.Application;
using RO.DevTest.Domain.Exception;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence.IoC;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using RO.DevTest.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RO.DevTest.WebApi;

public static class ProgramExtensions
{
    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder);
        
        return builder;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Configurações básicas
        builder.Services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });

        builder.Services.AddEndpointsApiExplorer();
        ConfigureSwagger(builder.Services);
        ConfigureMediatR(builder.Services);
        ConfigureAuthentication(builder);
        ConfigureDependencyInjection(builder);
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "RO.DevTest API", 
                Version = "v1",
                Description = "API protegida por JWT. Use o endpoint /auth/login para obter um token."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT no formato: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void ConfigureMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(ProgramExtensions).Assembly));
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void ConfigureDependencyInjection(WebApplicationBuilder builder)
    {
        builder.Services
            .InjectPersistenceDependencies(builder.Configuration)
            .InjectInfrastructureDependencies();
    }

    public static WebApplication BuildWebApplication(this WebApplicationBuilder builder)
    {
        var app = builder.Build();
        
        ApplyMigrations(app);
        ConfigureMiddlewarePipeline(app);
        
        return app;
    }

    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DefaultContext>();
        context.Database.Migrate();
    }

    private static void ConfigureMiddlewarePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RO.DevTest API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                
                if (contextFeature != null)
                {
                    var exception = contextFeature.Error;
                    var statusCode = exception switch
                    {
                        BadRequestException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    context.Response.StatusCode = statusCode;
                    
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = statusCode,
                        Message = exception.Message,
                        Details = exception is BadRequestException badRequest ? badRequest.ErrorDetails : null,
                        StackTrace = app.Environment.IsDevelopment() ? exception.StackTrace : null
                    });
                }
            });
        });

        app.MapControllers();
    }
}