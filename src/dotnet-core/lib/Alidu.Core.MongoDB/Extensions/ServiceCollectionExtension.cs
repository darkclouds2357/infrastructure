using Alidu.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MongoDB
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDbContext<TContextService>(this IServiceCollection serviceCollection, Action<IServiceProvider, MongoDbContextOptions> optionsAction, ServiceLifetime contextLifetime = ServiceLifetime.Scoped) where TContextService : MongoContext
        {
            if (serviceCollection is null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMongoDatabase), p => CreateMongoDatabase(p, optionsAction), contextLifetime));
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextService), contextLifetime));

            return serviceCollection;
        }

        public static IServiceCollection AddDbContext<TContextService, TContextImplementation>(this IServiceCollection serviceCollection, Action<IServiceProvider, MongoDbContextOptions> optionsAction, ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : MongoContext, TContextService
            where TContextService : IUnitOfWork
        {
            if (serviceCollection is null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMongoDatabase), p => CreateMongoDatabase(p, optionsAction), contextLifetime));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));

            return serviceCollection;
        }

        private static IMongoDatabase CreateMongoDatabase(IServiceProvider serviceProvider, Action<IServiceProvider, MongoDbContextOptions> optionsAction)
        {
            var options = new MongoDbContextOptions();

            optionsAction.Invoke(serviceProvider, options);

            var mongoClient = new MongoClient(options.Settings);

            var database = mongoClient.GetDatabase(options.DatabaseName);

            return database;
        }
    }
}
