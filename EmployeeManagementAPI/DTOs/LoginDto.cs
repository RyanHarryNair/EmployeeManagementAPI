namespace EmployeeManagementAPI.DTOs
{
    public class LoginDto // User only send email and password when loggin in
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
