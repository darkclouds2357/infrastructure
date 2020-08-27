using Alidu.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alidu.Core.ServiceHost
{
    public static class ConfigurationExtension
    {
        public static void UserServiceConfigure(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IConfiguration configuration, params object[] middlewares)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCorsz();
            app.UseSwaggerz(provider, configuration);

            if (middlewares?.Any() ?? false)
                foreach (var middleware in middlewares)
                {
                    app.UseMiddleware(middleware.GetType());
                }

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
