using Alidu.Core.ServiceHost;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;

namespace Alidu.xUnitTest
{
    public class TestServiceApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var configuration = DefaultProgram.GetConfiguration();

            builder
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) => DefaultProgram.BuildConfiguration(configurationBuilder))
                .UseStartup<TStartup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration);
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder();
        }
    }
}