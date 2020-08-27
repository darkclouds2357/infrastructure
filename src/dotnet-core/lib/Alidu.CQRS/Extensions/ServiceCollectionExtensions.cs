using Alidu.CQRS.Handler;
using Alidu.CQRS.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Alidu.CQRS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services, Func<IServiceProvider, IEventStoreProvider> eventStoreProvider, params Assembly[] assemblies)
        {
            services.AddScoped<IAggregateEventService, AggregateEventService>();
            services.AddScoped<IEventStoreService, EventStoreService>();

            services.AddScoped(eventStoreProvider);

            var integrationAssembly = new List<Assembly> { typeof(AggregateEventHandler<>).Assembly };
            integrationAssembly.AddRange(assemblies);

            services.AddMediatR(integrationAssembly.ToArray());
            return services;
        }
    }
}