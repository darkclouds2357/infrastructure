using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alidu.Core.ServiceHost;
using DomainServiceTemplate.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DomainServiceTemplate
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Contains('.') ? Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1) : Namespace;

        public static Task<int> Main(string[] args)
        {
            return DefaultProgram.MainProgram<Startup>(AppName, (context, sp) =>
            {
                var logger = sp.GetRequiredService<ILogger<DbContextSeed>>();
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                var contentRootPath = env.ContentRootPath;
                //new DbContextSeed().SeedAsync(context, logger, contentRootPath).Wait();
            }, args);
        }

    }
}
