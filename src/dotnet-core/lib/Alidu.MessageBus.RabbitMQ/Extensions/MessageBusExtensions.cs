using Alidu.Common;
using Alidu.Common.Interfaces;
using Alidu.MessageBus.RabbitMQ.Abstractions;
using Alidu.MessageBus.RabbitMQ.Connection;
using Alidu.MessageBus.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace Alidu.MessageBus.RabbitMQ
{
    public static class MessageBusExtensions
    {
        public static IMessageBus UseRabbitMQ(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>();
            var connection = serviceProvider.CreateRabbitMQPersistentConnection();
            var setting = serviceProvider.GetRequiredService<IOptions<MessageBusConfig>>();
            var messageTypes = serviceProvider.GetService<MessageTypeConfig>();
            var subsManager = serviceProvider.GetRequiredService<IMessageBusSubscriptionsManager>();
            var sp = serviceProvider.GetRequiredService<IServiceProvider>();

            var rabbitMQConfig = serviceProvider.GetRequiredService<IOptions<RabbitMQConfig>>().Value;
            var connectionOption = rabbitMQConfig.Connection;

            return new RabbitMQMessageBusManager(connection, logger, sp, subsManager, setting, messageTypes, rabbitMQConfig, connectionOption.RetryCount ?? 5);

        }

        private static IRabbitMQPersistentConnection CreateRabbitMQPersistentConnection(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            var connectionOption = serviceProvider.GetRequiredService<IOptions<RabbitMQConfig>>().Value.Connection;

            var factory = new ConnectionFactory()
            {
                DispatchConsumersAsync = true
            };
            if (!string.IsNullOrEmpty(connectionOption.Url))
                factory.Uri = new Uri(connectionOption.Url);
            else
            {
                if (!string.IsNullOrEmpty(connectionOption.HostName))
                    factory.HostName = connectionOption.HostName;
                if (!string.IsNullOrEmpty(connectionOption.UserName))
                    factory.UserName = connectionOption.UserName;
                if (!string.IsNullOrEmpty(connectionOption.Password))
                    factory.Password = connectionOption.Password;
                if (!string.IsNullOrEmpty(connectionOption.VirtualHost))
                    factory.VirtualHost = connectionOption.VirtualHost;
                if (connectionOption.Port.HasValue)
                    factory.Port = connectionOption.Port.Value;
            }

            var retryCount = connectionOption.RetryCount ?? 5;

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        }

        public static IServiceCollection AddMesageBusRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQOptions(configuration);
            services.AddSingleton<IMessageBus>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQMessageBus>>();
                var connection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var setting = sp.GetRequiredService<IOptions<MessageBusConfig>>();
                var messageTypes = sp.GetService<MessageTypeConfig>();
                var subsManager = sp.GetRequiredService<IMessageBusSubscriptionsManager>();
                var serviceProvider = sp.GetRequiredService<IServiceProvider>();

                var rabbitMQConfig = sp.GetRequiredService<IOptions<RabbitMQConfig>>().Value;
                var connectionOption = rabbitMQConfig.Connection;

                return new RabbitMQMessageBusManager(connection, logger, serviceProvider, subsManager, setting, messageTypes, rabbitMQConfig, connectionOption.RetryCount ?? 5);
            });
            return services;
        }

        private static IServiceCollection AddRabbitMQOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConfig>(configuration);
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var connectionOption = sp.GetRequiredService<IOptions<RabbitMQConfig>>().Value.Connection;

                var factory = new ConnectionFactory()
                {
                    DispatchConsumersAsync = true
                };
                if (!string.IsNullOrEmpty(connectionOption.Url))
                    factory.Uri = new Uri(connectionOption.Url);
                else
                {
                    if (!string.IsNullOrEmpty(connectionOption.HostName))
                        factory.HostName = connectionOption.HostName;
                    if (!string.IsNullOrEmpty(connectionOption.UserName))
                        factory.UserName = connectionOption.UserName;
                    if (!string.IsNullOrEmpty(connectionOption.Password))
                        factory.Password = connectionOption.Password;
                    if (!string.IsNullOrEmpty(connectionOption.VirtualHost))
                        factory.VirtualHost = connectionOption.VirtualHost;
                    if (connectionOption.Port.HasValue)
                        factory.Port = connectionOption.Port.Value;
                }

                var retryCount = connectionOption.RetryCount ?? 5;

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            return services;
        }
    }
}