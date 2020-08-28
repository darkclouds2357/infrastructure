using Alidu.Core.Startup.Warmup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Alidu.Core.Startup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
              where T : class, IStartupTask
              => services.AddTransient<IStartupTask, T>();

        public static IServiceCollection AddStartupTask<T, TOptions>(this IServiceCollection services, Action<IServiceProvider, TOptions> optionAction)
            where T : class, IStartupTask
            where TOptions : IWarmupOption
        {
            services.AddTransient<IStartupTask, T>(p => CreateStartUpTaskInstance<T, TOptions>(p, optionAction));
            return services;
        }

        public static IServiceCollection WarmupServiceStartup(this IServiceCollection services)
        {
            services.AddStartupTask<WarmupServicesStartupTask>().TryAddSingleton(services);
            return services;
        }

        private static T CreateStartUpTaskInstance<T, TOptions>(IServiceProvider serviceProvider, Action<IServiceProvider, TOptions> optionAction)
           where T : class, IStartupTask
           where TOptions : IWarmupOption
        {
            var optionInstance = (TOptions)Activator.CreateInstance(typeof(TOptions));
            optionAction.Invoke(serviceProvider, optionInstance);

            var startupTaskInstance = (T)Activator.CreateInstance(typeof(T), serviceProvider, optionInstance);
            return startupTaskInstance;
        }
    }
}