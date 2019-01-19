using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
namespace LoggingFunction
{
    public static class Function1
    {
        [FunctionName("Log")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Inject]IFunctionLogger log)
        {
            log.LogInformation("Log function","C# HTTP trigger function processed a request.");
           



            return (ActionResult)new OkObjectResult($"Hello, logging");
                
        }
    }
}
