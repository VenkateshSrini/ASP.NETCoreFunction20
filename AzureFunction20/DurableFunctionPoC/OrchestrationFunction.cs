using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DurableFunctionPoC.Model;
using DurableFunctionPoC.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace DurableFunctionPoC
{
    public static class OrchestrationFunction
    {
        [FunctionName("OrchestrationFunction")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("OrchestrationFunction_Hello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("OrchestrationFunction_Hello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("OrchestrationFunction_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("OrchestrationFunction_Hello")]
        public static string SayHello([ActivityTrigger] DurableActivityContext activityContext, ILogger log)
        {
            
            var name = activityContext.GetInput<string>();
            log.LogInformation($"Saying hello to {name}.");
            
            return $"Hello {name}!";
        }

        [FunctionName("OrchestrationFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,[Inject]ILeaveRepository respository,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("OrchestrationFunction", null);
            var leavereq = await req.Content.ReadAsAsync<Leave>();
            leavereq.WorkflowId = instanceId;
            leavereq.LeaveID = Guid.NewGuid();
            var result = await respository.AddLeave(leavereq, log);
            if (result != -1)
            {
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                await starter.TerminateAsync(instanceId, "Unable to add Leave to DB");
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(@"{""errorMessage"":""Unable to save leave to Db""}")

                };
            }
        }
    }
}