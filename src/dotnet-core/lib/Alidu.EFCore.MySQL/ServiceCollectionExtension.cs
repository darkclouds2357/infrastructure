using Alidu.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace Alidu.EFCore.MySQL
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContext<TContext>(this IServiceCollection services, string connectionString, ServerType serverType = ServerType.MySql) where TContext : EFContext
        {
            services.AddEntityFrameworkMySql();
            services.AddDbContext<IUnitOfWork, TContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var mySqlVersion = new Version(configuration.GetValue<string>("DbVersion"));
                options.UseMySql(configuration.GetConnectionString(connectionString), b =>
                {
                    b.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    b.ServerVersion(mySqlVersion, serverType);
                    b.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
                options.UseInternalServiceProvider(serviceProvider);
            });
            return services;
        }
    }
}