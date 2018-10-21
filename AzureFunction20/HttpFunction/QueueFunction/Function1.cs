using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QueueFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "zzzz")]string myQueueItem, ILogger log, ICollector<string> collector)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
       

        }
    }
}
