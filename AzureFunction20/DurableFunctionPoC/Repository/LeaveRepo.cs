using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using DurableFunctionPoC.Model;
using Microsoft.Extensions.Logging;
using Npgsql;
namespace DurableFunctionPoC.Repository
{
    public class LeaveRepo : ILeaveRepository
    {
        string _connectionString;
        
        public LeaveRepo (string connection)
        {
            _connectionString = connection;
        }
        private DbConnection createNewConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
        public Task<int> AddLeave(Leave leave, ILogger log)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteLeave(int employeeID, ILogger log)
        {
            throw new NotImplementedException();
        }

        public Task<Leave> GetLeave(int EmployeeId, string OrchestrationId, ILogger log)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateLeave(Leave leave, ILogger log)
        {
            throw new NotImplementedException();
        }

        

    }
}
