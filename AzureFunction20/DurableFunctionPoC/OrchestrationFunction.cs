using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
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
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,  [Inject]ILeaveRepository respository, ILogger log)
        {
            Leave leave = context.GetInput<Leave>();
            await context.CallActivityAsync<string>("ApproveLeave", leave);
            using (var cts = new CancellationTokenSource())
            {
                var timeOut = context.CurrentUtcDateTime.AddMinutes(5);
                var timeOutTask = context.CreateTimer(timeOut, cts.Token);
                var approvalTask = context.WaitForExternalEvent<string>("ApproveLeave");
                Task result = Task.WhenAny(approvalTask, timeOutTask);
                if(result == approvalTask)
                {
                    cts.Cancel();
                    return approvalTask.Result;
                }
                else
                {
                    leave.LeaveStatus = LeaveStatus.Approved;
                    var updateResult = await respository.UpdateLeave(leave, log);
                    return "Approved";
                }
            }



        }

        [FunctionName("ApproveLeave")]
        public static string ApproveLeave([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient, [Inject]ILeaveRepository respository,
            ILogger log)
        {
            
            //var name = orchestrationClient.Get
            //log.LogInformation($"Saying hello to {name}.");
            
            return $"Hello !";
        }

        [FunctionName("ApplyLeave")]
        public static async Task<HttpResponseMessage> ApplyLeave_StartWF(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,[Inject]ILeaveRepository respository,
            ILogger log)
        {
            // Function input comes from the request content.
            
            var leavereq = await req.Content.ReadAsAsync<Leave>();
            //leavereq.WorkflowId = instanceId;
            leavereq.LeaveID = Guid.NewGuid();
            var result = await respository.AddLeave(leavereq, log);
            if (result != -1)
            {
                string instanceId = await starter.StartNewAsync("OrchestrationFunction", leavereq);
                leavereq.WorkflowId = instanceId;
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                //await starter.TerminateAsync(instanceId, "Unable to add Leave to DB");
                return new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(@"{""errorMessage"":""Unable to save leave to Db""}")

                };
            }
        }
    }
}