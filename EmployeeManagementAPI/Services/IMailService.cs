using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Services
{
    public interface IMailService // Interface is a method without a body {}
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
