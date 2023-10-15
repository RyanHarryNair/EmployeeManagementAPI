using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        public MailController(IMailService mailService) // In the case of Interface, inorder to do DI, you need to configure the DI in Program.cs, Line 24 
        {
            this.mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request) // we need to send attachment to.. so we use [FromForm]. Not [FromBody]. We are sending IForm of Attachments
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Mail Sent"
                });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
