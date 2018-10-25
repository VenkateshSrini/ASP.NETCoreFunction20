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
    public static class FileWriterFunction
    {
        [FunctionName("FileWriterFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            ILogger log, [FileAccess]ICollector<FileContent> collector)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["FileName"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            string content = data?.Content;
            collector.Add(new FileContent
            {
                FileName = name,
                Content = content
            });

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
