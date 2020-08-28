using Alidu.Common;
using Alidu.Core.Startup;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alidu.Core.ServiceHost
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddStartupServices(this IServiceCollection services, IConfiguration configuration, Type startUpType)
        {
            services.WarmupServiceStartup()
                .AddCorsService(configuration)
                .AddApiVersioningService()
                .AddOptionConfigurationServices()
                .AddSwaggerGenService(configuration, startUpType.Assembly)
                .AddHttpContextAccessor()
                .AddAutoMapper(startUpType)
                .AddRequestHeader();

            services.AddControllers();
            services
                .AddMvcCore(option => { })
                .AddNewtonsoftJson(options => { })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHealthChecks()
                .ForwardToPrometheus();
            return services;
        }

        private static IServiceCollection AddApiVersioningService(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer().AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.DefaultApiVersion = new ApiVersion(1, 0); // specify the default api version
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new MediaTypeApiVersionReader("api-version"); // read the version number from the accept header
            });

            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'V'VVV";
            });
            return services;
        }

        private static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration configuration)
        {
            var domains = configuration["Cors:Domains"].Split(',').ToArray();
            if (domains.Length > 0)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .WithOrigins(domains)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
                });
            }

            return services;
        }

        private static IServiceCollection AddOptionConfigurationServices(this IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerGenService(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider()
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var apiVersion in provider.ApiVersionDescriptions)
                {
                    // ConfigureVersionedDescription(options, apiVersion);
                    options.SwaggerDoc(apiVersion.GroupName, new OpenApiInfo()
                    {
                        Title = $"API - version {apiVersion.ApiVersion}",
                        Version = apiVersion.ApiVersion.ToString(),
                        Description = apiVersion.IsDeprecated ? $"API - DEPRECATED" : "API",
                    });
                }
                options.AddSecurityDefinition(AuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = AuthenticationDefaults.AuthorizationHeaderName,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthenticationDefaults.MSXSchemeName
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = AuthenticationDefaults.AuthenticationScheme
                            },
                            Scheme = AuthenticationDefaults.MSXSchemeName,
                            Name = AuthenticationDefaults.AuthorizationHeaderName,
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                var hostEnvironment = Environment.GetEnvironmentVariable("HOST_ENV");
                if (hostEnvironment == "traefik")
                {
                    options.DocumentFilter<TraefikPrefixInsertDocumentFilter>(Environment.GetEnvironmentVariable("SERVICE_PREFIX"));
                }

                //// Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}