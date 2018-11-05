using DurableFunctionPoC.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionPoC.Repository
{
    public interface ILeaveRepository
    {
        Task<Leave> GetLeave(int EmployeeId, string OrchestrationId, ILogger log);
        Task<int> AddLeave(Leave leave, ILogger log);
        Task<bool> UpdateLeave(Leave leave, ILogger log);
        Task<bool> DeleteLeave(int employeeID, ILogger log);
    }
}
