using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Alidu.EFCore.MySQL
{
    public abstract class EFDbContextDesignFactory<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : EFContext

    {
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public abstract string ConnectionName { get; protected set; }

        public TContext CreateDbContext(string[] args)
        {
            var configuration = GetConfigurationRoot();
            var builder = new DbContextOptionsBuilder<TContext>();
            var connectionString = configuration.GetConnectionString(ConnectionName);

            builder.UseMySql(connectionString, b => b.MigrationsAssembly(this.GetType().Assembly.FullName));

            return Activator.CreateInstance(typeof(TContext), builder.Options) as TContext;
        }

        public abstract IConfigurationRoot GetConfigurationRoot();
    }
}