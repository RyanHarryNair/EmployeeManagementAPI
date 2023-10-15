namespace EmployeeManagementAPI.DTOs
{
    public class UserDto // We will not be sharing 'Models' everytime. We only share 'Dtos' with User. For eg, Facebook users no need to see created date, updated by, etc and stuffs. Only Auditers are interested in all these data.
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int DepartmentId { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime StartDate { get; set; }
    }
}

//  Most of the 'UserDto' data are coming from 'UserModel'
