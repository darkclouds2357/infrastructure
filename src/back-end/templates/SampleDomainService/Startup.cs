using Alidu.Core.Domain.Interfaces;
using Alidu.Core.ServiceHost;
using Alidu.CQRS;
using Alidu.CQRS.ExecptionHandler;
using Alidu.MessageBus;
using Alidu.MessageBus.RabbitMQ;
using Alidu.MessageBus.RabbitMQ.Connection;
using Alidu.MessageBus.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleDomainService.Application.Commands;
using SampleDomainService.Application.Events;
using SampleDomainService.Data;
using System;
using System.Collections.Generic;

namespace SampleDomainService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCQRS(sp =>
                {
                    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
                    var mapper = sp.GetRequiredService<IMapper>();
                    return new EventStoreProvider(unitOfWork, mapper);
                })
                .AddStartupServices(Configuration, typeof(Startup))
                .AddMessageBus<RabbitMQConfig>(Configuration, sp => sp.UseRabbitMQ(), GetMessageTypeConfig());

            AddHandler(services);
        }

        protected void AddHandler(IServiceCollection services)
        {
            services.AddTransient<CreateSampleCommandHandler>();
        }

        protected MessageTypeConfig GetMessageTypeConfig()
        {
            return new MessageTypeConfig
            {
                DispatcherMessageTypes = new Dictionary<Type, string>
                {
                    [typeof(SampleCreatedEvent)] = SampleEventEnum.NEW_SAMPLE_CREATED_EVENT
                },
                ListenerMessageTypes = new Dictionary<Type, string>
                {
                    [typeof(CreateSampleCommand)] = SampleCommandEnum.CREATE_NEW_SAMPLE_COMMAND
                }
            };
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UserServiceConfigure(env, provider, Configuration, typeof(CQRSExceptionMiddleware));
        }
    }
}