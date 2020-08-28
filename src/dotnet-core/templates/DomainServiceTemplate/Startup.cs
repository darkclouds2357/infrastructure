using System;
using System.Collections.Generic;
using System.Linq;
using Alidu.CQRS;
using Alidu.MessageBus;
using Alidu.MessageBus.RabbitMQ;
using Alidu.Core.ServiceHost;
using Alidu.CQRS.ExecptionHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Alidu.Core.Domain.Interfaces;
using AutoMapper;
using SampleDomainService.Data;
using Alidu.MessageBus.RabbitMQ.Connection;

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
                .AddMessageBus<RabbitMQConfig>(Configuration, sp => sp.UseRabbitMQ(), new Alidu.MessageBus.Settings.MessageTypeConfig
                {

                });
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UserServiceConfigure(env, provider, Configuration, typeof(CQRSExceptionMiddleware));
        }
    }
}
