using Alidu.CQRS.Handler;
using Alidu.CQRS.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Alidu.CQRS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services, Func<IServiceProvider, IEventStoreProvider> eventStoreProvider, params Assembly[] assemblies)
        {
            services.AddScoped<IIntegrationMessageService, IntegrationMessageService>();
            services.AddScoped<IEventStoreService, EventStoreService>();

            services.AddScoped(eventStoreProvider);

            var integrationAssembly = new List<Assembly> { typeof(IntegrationEventHandler<>).Assembly };
            integrationAssembly.AddRange(assemblies);

            services.AddMediatR(integrationAssembly.ToArray());
            return services;
        }
    }
}
