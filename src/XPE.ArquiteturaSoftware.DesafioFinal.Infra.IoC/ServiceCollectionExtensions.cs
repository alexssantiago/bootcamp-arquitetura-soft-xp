using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Serilog;
using Serilog.Debugging;
using System.Data;
using System.Reflection;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Validators;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Repositories;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        SelfLog.Enable(Console.Error);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(
                serverUrl: configuration.GetConnectionString("Seq") ?? "http://localhost:5341"
            )
            .CreateLogger();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                ctx.ProblemDetails.Title ??= "Unexpected error";
            };
        });

        services.AddExceptionHandler(options =>
        {
            options.ExceptionHandler = async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                var problem = new ProblemDetails
                {
                    Title = "Unhandled exception",
                    Detail = ex?.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            };
        });

        services.AddRouting(options => options.LowercaseUrls = true);

        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

        services.AddScoped<IDbConnection>(_ =>
            new MySqlConnection(configuration.GetConnectionString("Default")));

        services.AddStackExchangeRedisCache(o =>
        {
            o.Configuration = configuration["Redis:Configuration"];
        });

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<IProductService, ProductService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bootcamp: Arquiteto(a) de Software — Desafio Final",
                Version = "v1",
                Description = "API para fins acadêmicos — Autor: Alexsander Santiago Sillva Alves."
            });

            c.EnableAnnotations();

            var basePath = AppContext.BaseDirectory;
            foreach (var asm in new[] {
                Assembly.GetExecutingAssembly(),
                typeof(ProductService).Assembly
            })
            {
                var xml = Path.Combine(basePath, $"{asm.GetName().Name}.xml");
                if (File.Exists(xml))
                    c.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }
        });

        return services;
    }
}