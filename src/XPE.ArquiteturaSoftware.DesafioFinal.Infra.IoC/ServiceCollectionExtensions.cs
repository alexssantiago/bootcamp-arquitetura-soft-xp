using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MySqlConnector;
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