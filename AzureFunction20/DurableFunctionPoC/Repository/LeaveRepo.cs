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
        private NpgsqlConnection createNewConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
        public async Task<int> AddLeave(Leave leave, ILogger log)
        {
            log.LogInformation("DB Insert started");
            var connection = createNewConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO public.leavetbl(
            leaveid, empployeeid, employeename, type, status, reason, wfid)
    VALUES(@leaveid, @empployeeid, @employeename, @type, @status, @reason, @wfid);";
            command.Parameters.AddWithValue("@leaveid", leave.LeaveID);
            command.Parameters.AddWithValue("@empployeeid", leave.EmployeeID);
            command.Parameters.AddWithValue("@employeename", leave.EmployeeName);
            command.Parameters.AddWithValue("@type", (int)leave.Type);
            command.Parameters.AddWithValue("@status", (int)leave.LeaveStatus);
            command.Parameters.AddWithValue("@reason", leave.Reason);
            command.Parameters.AddWithValue("@wfid", leave.WorkflowId);
            try
            {
                await connection.OpenAsync();
                var noOfRecords = await command.ExecuteNonQueryAsync();
                log.LogInformation("DB Insert Completed");
                return noOfRecords;
                
            }
            catch(Exception ex)
            {
                log.LogError($"DB Insert failed with {ex.Message} stack trace {ex.StackTrace}");
                return -1;
            }




        }

        public async Task<bool> DeleteLeave(int employeeID, ILogger log)
        {
            log.LogInformation("DB Delete started");
            var connection = createNewConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM public.leavetbl WHERE empployeeid=@empployeeid;";
            command.Parameters.AddWithValue("@empployeeid", employeeID);
            try
            {
                await connection.OpenAsync();
                var noOfRecords = await command.ExecuteNonQueryAsync();
                log.LogInformation("DB Delete Completed");
                return true;

            }
            catch (Exception ex)
            {
                log.LogError($"DB Delete failed with {ex.Message} stack trace {ex.StackTrace}");
                return false;
            }
        }

        public async Task<Leave> GetLeave(int EmployeeId, string LeaveId, ILogger log)
        {
            log.LogInformation("DB Select started");
            var connection = createNewConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT leaveid, empployeeid, employeename, type, status, reason, wfid
  FROM public.leavetbl WHERE empployeeid=@empployeeid AND leaveid=@leaveid;";
            command.Parameters.AddWithValue("@empployeeid", EmployeeId);
            command.Parameters.AddWithValue("@leaveid", LeaveId);
            try
            {
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows && reader.Read() )
                {
                    Leave leave = new Leave();
                    leave.LeaveID = Guid.Parse(reader.GetString(0));
                    leave.EmployeeID = reader.GetInt32(1);
                    leave.EmployeeName = reader.GetString(2);
                    leave.Type = (LeaveType) Enum.ToObject(typeof(LeaveType),reader.GetInt32(3)) ;
                    leave.LeaveStatus = (LeaveStatus)Enum.ToObject(typeof(LeaveStatus), reader.GetInt32(4));
                    leave.Reason = reader.GetString(5);
                    leave.WorkflowId = reader.GetString(6);
                    return leave;

                }
                log.LogInformation("DB Select Completed");
                return null;

            }
            catch (Exception ex)
            {
                log.LogError($"DB Select failed with {ex.Message} stack trace {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> UpdateLeave(Leave leave, ILogger log)
        {
            log.LogInformation("DB Update started");
            var connection = createNewConnection();
            var command = connection.CreateCommand();
            command.CommandText = @"UPDATE public.leavetbl SET type=@type, status=@status, reason=@reason, wfid = @wfid  WHERE leaveid=@leaveid;";
            command.Parameters.AddWithValue("@type", (int)leave.Type);
            command.Parameters.AddWithValue("@status", (int)leave.LeaveStatus);
            command.Parameters.AddWithValue("@reason", leave.Reason);
            command.Parameters.AddWithValue("@wfid", leave.WorkflowId);
            command.Parameters.AddWithValue("@leaveid", leave.LeaveID.ToString());
            try
            {
                await connection.OpenAsync();
                var noOfRecords = await command.ExecuteNonQueryAsync();
                log.LogInformation("DB Update Completed");
                return true;

            }
            catch (Exception ex)
            {
                log.LogError($"DB Update failed with {ex.Message} stack trace {ex.StackTrace}");
                return false;
            }
        }

        

    }
}
