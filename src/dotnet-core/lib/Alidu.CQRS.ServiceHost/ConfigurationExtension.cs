using Alidu.CQRS.ExecptionHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alidu.CQRS.ServiceHost
{
    public static class ConfigurationExtension
    {
        public static void UserCQRSConfigure(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            app.UseMiddleware<CQRSExceptionMiddleware>();
            app.UseCorsz();
            app.UseSwaggerz(provider, configuration);
        }

        public static void UseCorsz(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
        }

        public static void UseSwaggerz(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersion in provider.ApiVersionDescriptions
                    .OrderBy(version => version.ToString()))
                {
                    c.SwaggerEndpoint(
                        $"swagger/{apiVersion.GroupName.ToUpperInvariant()}/swagger.json", apiVersion.GroupName.ToUpperInvariant()
                    );
                }

                c.RoutePrefix = string.Empty;
            });
        }
    }
}
