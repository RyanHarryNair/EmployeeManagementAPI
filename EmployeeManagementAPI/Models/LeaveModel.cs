using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class LeaveModel : BaseModel
    {
        [Key]
        public int LeaveId { get; set; }
        public int UserId { get; set; }                  // 'Id' is primary key in 'UserModel' and a foreign key in 'LeaveModel' (named as 'UserId'). We can use this to combine the 2 tables.
        public DateTime LStartDate { get; set; }
        public DateTime LEndDate { get; set; }
        public int Count { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
    }
}
