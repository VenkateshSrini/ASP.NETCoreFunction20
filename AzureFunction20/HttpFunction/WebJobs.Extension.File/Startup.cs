using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using WebJobs.Extension.File.Attribute;

[assembly: WebJobsStartup(typeof(WebJobs.Extension.File.Startup))]
namespace WebJobs.Extension.File
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            
            builder.AddExtension<FileAccessExtension>();            
        }
    }
}
