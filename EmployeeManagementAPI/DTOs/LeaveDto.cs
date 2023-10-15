namespace EmployeeManagementAPI.DTOs
{
    public class LeaveDto
    {
        public int LeaveId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LStartDate { get; set; }
        public DateTime LEndDate { get; set; }
        public int Count { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
    }
}
