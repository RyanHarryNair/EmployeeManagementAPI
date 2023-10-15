using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class DepartmentModel : BaseModel
    {
        [Key]
        public int DeptId { get; set; }
        public string DeptName { get; set; }
    }
}
