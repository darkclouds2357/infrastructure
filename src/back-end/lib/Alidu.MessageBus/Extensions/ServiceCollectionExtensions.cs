﻿using Alidu.MessageBus.InMemory;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Alidu.MessageBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus<TConnectionConfig>(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IMessageBus> messageBusProvider, MessageTypeConfig messageTypeConfig = null) where TConnectionConfig : class
        {
            services.Configure<TConnectionConfig>(configuration);
            services.AddMessageBusOptions(configuration, messageTypeConfig)
                .AddInMemoryMessageBusSubscriptions()
                .AddSingleton(messageBusProvider);

            return services;
        }

        public static IServiceCollection AddMessageBusOptions(this IServiceCollection services, IConfiguration configuration, MessageTypeConfig messageTypeConfig = null)
        {
            services.Configure<MessageTypeConfig>(configuration);

            if (messageTypeConfig != null)
                services.AddSingleton(messageTypeConfig);
            return services;
        }

        public static IServiceCollection AddInMemoryMessageBusSubscriptions(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBusSubscriptionsManager>(sp =>
            {
                var messageTypes = sp.GetService<MessageTypeConfig>();

                var setting = sp.GetRequiredService<IOptions<MessageBusConfig>>().Value;

                return new InMemoryMessageBusSubscriptionsManager(messageTypes?.ListenerMessageTypes);
            });
            return services;
        }

        public static IServiceCollection AddInMemoryMessageBus(this IServiceCollection services, IConfiguration configuration, MessageTypeConfig messageTypeConfig = null)
        {
            services.AddMessageBusOptions(configuration, messageTypeConfig);
            services.AddInMemoryMessageBusSubscriptions();

            services.AddSingleton<IMessageBus>(sp =>
            {
                var setting = sp.GetRequiredService<IOptions<MessageBusConfig>>();
                var messageTypes = sp.GetService<MessageTypeConfig>();
                var subsManager = sp.GetRequiredService<IMessageBusSubscriptionsManager>();
                var serviceProvider = sp.GetRequiredService<IServiceProvider>();
                var inMemoryQueue = sp.GetRequiredService<InMemoryQueue>();
                return new InMemoryMessageBusManager(serviceProvider, subsManager, setting, messageTypes, inMemoryQueue);
            });
            services.AddSingleton<InMemoryQueue>();
            return services;
        }
    }
}