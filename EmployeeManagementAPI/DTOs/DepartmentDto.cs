namespace EmployeeManagementAPI.DTOs
{
    public class DepartmentDto
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
