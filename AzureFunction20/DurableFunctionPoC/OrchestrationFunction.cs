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
            log.LogInformation("Executing workflow " + context.InstanceId);

            Leave leave = context.GetInput<Leave>();
            leave.WorkflowId = context.InstanceId;
            //leave.WorkflowId = context.InstanceId;
            ////updating workflowid
            //var res = await respository.UpdateLeave(leave, log);
            //await context.CallActivityAsync<string>("ApproveLeave", context.InstanceId);
            if (leave.LeaveStatus == LeaveStatus.Applied)
            {
                using (var cts = new CancellationTokenSource())
                {
                    var timeOut = context.CurrentUtcDateTime.AddMinutes(5);
                    var timeOutTask = context.CreateTimer(timeOut, cts.Token);
                    var approvalTask = context.WaitForExternalEvent<string>("ApproveLeaveEvent");


                    var result = await Task.WhenAny(approvalTask, timeOutTask);
                    if (result == approvalTask)
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
            else
            {
                return "Nothing to process";
            }



        }

        [FunctionName("ApproveLeave")]
        public static async Task<string> ApproveLeave([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient, [Inject]ILeaveRepository respository,
            ILogger log)
        {
            string status = default(string);
            var leavereq = await req.Content.ReadAsAsync<Leave>();
            var pendingLeave = await respository.GetLeave(leavereq.EmployeeID, leavereq.LeaveID.ToString(),log);
            pendingLeave.LeaveStatus = LeaveStatus.Approved;

            //var pendingLeave = await respository.GetLeave(leavereq.EmployeeID, leavereq.LeaveID.ToString(),log);
            //pendingLeave.LeaveStatus = LeaveStatus.Approved;
            if (await respository.UpdateLeave(pendingLeave, log))
            {
                status = "Approved";
            }
            else
                status = "Rejected";

            
            //var name = orchestrationClient.Get
            //log.LogInformation($"Saying hello to {name}.");
            await orchestrationClient.RaiseEventAsync(pendingLeave.WorkflowId, "ApproveLeaveEvent", status);
            return status;
        }

        [FunctionName("ApplyLeave")]
        public static async Task<HttpResponseMessage> ApplyLeave_StartWF(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,[Inject]ILeaveRepository respository,
            ILogger log)
        {
            // Function input comes from the request content.
            
            var leavereq = await req.Content.ReadAsAsync<Leave>();
            
            leavereq.WorkflowId = string.Empty;
            leavereq.LeaveID = Guid.NewGuid();
            leavereq.LeaveStatus = LeaveStatus.Applied;
            var result = await respository.AddLeave(leavereq, log);
            if (result != -1)
            {
                string instanceId = await starter.StartNewAsync("OrchestrationFunction", leavereq);
                leavereq.WorkflowId = instanceId;
                var res = await respository.UpdateLeave(leavereq, log);


                
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