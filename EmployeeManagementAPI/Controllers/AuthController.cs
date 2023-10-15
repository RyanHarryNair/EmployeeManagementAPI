using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Helpers;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeContext _employeeContext;
        private readonly IConfiguration _configuration;
        public AuthController(EmployeeContext employeeContext, IConfiguration configuration)
        {
            _employeeContext = employeeContext;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody]LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Login and Password fields is empty!"
                    });
                }
                var user = await _employeeContext.UserModels.FirstOrDefaultAsync(a => a.Email == loginDto.Email); // compare 'Email' properties of both 'model' and 'Dto' object to check if user is present in the database or not
                if (user == null)
                {
                    return NotFound(new
                    {
                        Status = 404,
                        Message = "User Not Found!"
                    });
                }
                else // If there is a user, then I need to check password also
                {   
                    if (!EncDescPassword.VerifyHashPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt)) /* ! Logical Not operator (Inverse):- In the context of password verification, this usage implies that if the password verification fails (returns false), the if condition evaluates to true, indicating that the entered password is incorrect. 
                                                                                                                       * This method sends a boolean (true or false). 'Password' from Dto object 'loginDto'; 'PasswordHash' & 'PasswordSalt' from the database model object 'user' */
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Wrong Password!"
                        });
                        
                    }
                    string token = CreateJwtToken(user);  // Create Token on Login Success     // Line 76 :- private string CreateJwtToken(UserModel user); Only 'UserModel' data type can be passed inside
                    return Ok(new
                    {
                        Status = 200,
                        Message = "Login Success!",
                        Token = token                                // Line 85:-  var token = new JwtSecurityToken(); When log in is successfull, pass token
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }     

        // Payload, Key, Credentials(Verify Signature), Token, Expiry Time,
        private string CreateJwtToken(UserModel user)                //This returns a string of token @ line 91 (return jwt;). Pass usermodel inside the method becasue usermodel has all your info & we need to ulilize it @ Line 80 "new Claim("Email", user.Email),"
        {
            List<Claim> claimsList = new List<Claim>                //  created generic list of type claim. Payload is called as 'claims' in .Net
            {
                new Claim("Email", user.Email),                  // claimsList.Add(); if I want to add one by one. If you want to add multiple entries in generic list, use this way 
                new Claim("FullName", user.FirstName + " " + user.LastName),           // Passing 'Email' and 'FirstName' properties as Payload 
                new Claim("RoleID", user.RoleId.ToString()),
                new Claim("UserID", user.Id.ToString()),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value));                // Security Key... Pass byte[] Key... Pass this same key in startup.cs.services.AddAuthentication & appsettings.json
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);                       // Credentials. Pass Key and algorithm inside the class
            var token = new JwtSecurityToken(                                                                            // Token. Inside this we pass an array of objects
                claims: claimsList,                                                         // Passed ClaimsList inside
                expires: DateTime.Now.AddDays(1),                                          // Passed expiry time; AddDays(1): token valid for 1 day
                signingCredentials: credentials);                                          // Passed Credentials

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);                     // this class helps to write it into a token. something like 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c'
            return jwt;                                                          //After token is created, we have to validate it (Who can use that controller). go to startup class 
        }

        [HttpPost("reset")]
        public async Task<ActionResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto) //User will pass email, old and new password
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Fields is empty!"
                    });
                }
                else
                {
                    var user = await _employeeContext.UserModels.FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email); // By email I can check if user exist or not
                    if (user == null)
                    {
                        return NotFound(new
                        {
                            Status = 404,
                            Message = "User Not Found!"
                        });
                    }
                    else
                    {
                        if (!EncDescPassword.VerifyHashPassword(resetPasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt)) // check if password is correct
                        {
                            return BadRequest(new
                            {
                                Status = 400,
                                Message = "Wrong Password!"
                            });

                        }
                        EncDescPassword.CreateHashPassword(resetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt); // Reset PAssword
                        user.PasswordHash = passwordHash; // Model properties 'PasswordHash' & 'PasswordSalt' updated via model instance
                        user.PasswordSalt = passwordSalt;
                        _employeeContext.Entry(user).State = EntityState.Modified; //Only 'Model' 'object' can be passed inside 'Entry'. (Not 'Dto' 'objects')
                        await _employeeContext.SaveChangesAsync();
                        return Ok(new
                        {
                            StatusCode = 200,
                            Message = "Password Updated"
                        });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
