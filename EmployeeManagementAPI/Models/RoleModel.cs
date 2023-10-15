using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class RoleModel : BaseModel
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
