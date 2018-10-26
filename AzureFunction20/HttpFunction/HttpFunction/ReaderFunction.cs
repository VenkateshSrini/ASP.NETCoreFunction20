using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebJobs.Extension.File.Attribute;
using WebJobs.Extension.File;

namespace HttpFunction
{
    public static class ReaderFunction
    {
        [FunctionName("ReaderFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] FileContent Item, 
            [FileAccess(FileName = "{FileName}")]string content, 
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return (ActionResult)new OkObjectResult(new FileContent {
                Content = content
                
            });
                
        }
    }
}
