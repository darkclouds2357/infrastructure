using Alidu.Core.Domain.Interfaces;
using Alidu.Core.ServiceHost;
using Alidu.CQRS;
using Alidu.MessageBus;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleDomainService;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleDomainServiceTest.Setup
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services
                   .AddCQRS(sp =>
                   {
                       var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
                       var mapper = sp.GetRequiredService<IMapper>();
                       return new FakeEventStoreProvider();
                   })
                   .AddStartupServices(Configuration, typeof(Startup))
                   .AddInMemoryMessageBus(Configuration, GetMessageTypeConfig());
            AddHandler(services);
        }
    }
}
