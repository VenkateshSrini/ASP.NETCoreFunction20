using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunctionPoC.Model
{
    public enum LeaveStatus
    {
        Applied=0,
        Approved=1,
        Rejected = -1
    }
    public enum LeaveType
    {
        Sick =0,
        Casual = 1,
        Personal = 2
    }
    public class Leave
    {
        public int EmployeeID { get; }
        public string EmployeeName { get; }
        public LeaveType Type { get; }
        public LeaveStatus LeaveStatus { get; set; }
        public string Reason { get; set; }
        public Leave(int id, string name, LeaveType leaveType, string reason)
        {
            EmployeeID = id;
            EmployeeName = name;
            Type = leaveType;
            Reason = reason;
            LeaveStatus = LeaveStatus.Applied;
        }
    }
}
