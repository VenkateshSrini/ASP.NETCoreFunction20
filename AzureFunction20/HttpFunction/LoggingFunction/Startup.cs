using LoggingFunction;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.File;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace LoggingFunction
{
    public class Startup : IWebJobsStartup
    {
        public Startup() => BuildConfiguration();
        private void BuildConfiguration()
        {
            //Configuration = new ConfigurationBuilder()
            //                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //                   .AddEnvironmentVariables()
            //                   .Build();
             
            Log.Logger = new LoggerConfiguration()
                           .WriteTo.File("log.txt")
                           .CreateLogger();

        }
        public void Configure(IWebJobsBuilder builder) => builder.AddDependencyInjection(ConfigureServices);
        
        private void ConfigureServices(IServiceCollection obj)
        {
            obj.AddLogging(
                (logbuilder) => logbuilder.AddSerilog()
                );
            obj.AddScoped<IFunctionLogger, FunctionLogger>();
           
        }
    }
}
