using DurableFunctionPoC.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(DurableFunctionPoC.Startup))]
namespace DurableFunctionPoC
{
    public class Startup : IWebJobsStartup
    {
        private IConfiguration Configuration;
        private void BuildConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                         .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
        }
        public void Configure(IWebJobsBuilder builder) =>
            builder.AddDependencyInjection(ConfigureServices);

        private void ConfigureServices(IServiceCollection obj)
        {
            var connection = Configuration.GetConnectionString("SqlConnectionString");
            obj.AddSingleton<ILeaveRepository, LeaveRepo>((serviceProvider) => new LeaveRepo(connection)
             );
        }
    }
}
