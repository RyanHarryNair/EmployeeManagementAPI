using AutoMapper;
using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Helpers;
using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")] //Controller name is 'Employee'. Check Line 16 : public class EmployeeController : ControllerBase
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _userContext; //declaring variable named '_userContext' of type 'EmployeeContext'. 
        private readonly IMapper _mapper;                // created a variable named '_mapper' of type 'IMapper'. Create instance using DI // Dtos
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _env;       // creates a web host enviroment for storing images. It stored the project folders.

        public EmployeeController(EmployeeContext context, IMapper mapper, IMailService mailService, IWebHostEnvironment env) // Using constructor & DI created instance 'context' to access properties inside the class 'EmployeeContext'. check like 24 '_userContext.UserModels.ToListAsync();;'
        {
            _userContext = context;             //// initilizing the variable in the constructor. Constructor is responsible for initilizing the variable. Store 'Context' inside variable '_userContext'
            _mapper = mapper;
            _mailService = mailService;
            _env = env;
        }

        // Get the user details from the database
        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetAllEmployees()             // ActionResult provides us 'return Ok' and all that // Pass the classname 'UserDto' inside 'ActionResult'
        {
            var employeeList = await _userContext.UserModels.ToListAsync();    // We are converting 'UserModel' to 'UserDto' in line 34 becasue we need to show the 'UserDTo' data to user (Get Request)
            var mappedEmployeeList = _mapper.Map<List<UserDto>>(employeeList); // This line uses '_mapper' (an instance of object mapper 'AutoMapper') to map the 'employeeList' to a list of 'UserDto' objects. 
            return Ok(new ResponseDto<UserDto>                                 // a new instance of 'ResponseDto<UserDto>' is created inline using object initializer syntax. 
            {                                                                 // other than saying just 'return (mappedEmployeeList);', create a habit of creating 'new' object to pass extra details
                StatusCode = 200,
                Message = "Success",
                Result = mappedEmployeeList
            });                                                               // when status code 200 or success, it gives an Ok response while getting the employee list from the database and send to the user// 'OK' comes from ActionResult(the type is Action result, not void, string etc)                                   
        }

        // Use to view the enire record for particular employee by ID
        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetEmployee(int id)        // id will be coming from route
        {
            var user = await _userContext.UserModels.FindAsync(id);         // FindAsync() finds your key value
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found!"
                });
            }
            var mappedUser = _mapper.Map<UserDto>(user);                     // We are converting 'UserModel' to 'UserDto' becasue we need to show the 'UserDTo' data to user (Get Request)
            return Ok(new 
            {
                StatusCode=200,
                Message="Success",
                Result=mappedUser
            });
        }

        // Adding Employee
        // POST api/<EmployeeController>
        [HttpPost("add")]                                                     //// Post in the database; route/endpoint is 'add'
        //public async Task<ActionResult<UserDto>> AddEmployee([FromBody] UserDto userDtoObj) // Get the data from 'formbody' of type 'UserDto' which is from UI, Postman or Angular; kind of data is 'UserDto'
        public async Task<ActionResult<UserDto>> AddEmployee()
        {
            IFormFileCollection req = Request.Form.Files;                           // 'IFormFIleCollection' is an Interface provided by ASP.Net core Http, which collects your response of a file. If you receive any files in the http, it will collect it.
            var files = req;                                                        // The collected files are store inside var 'files'
            var userDtoString = Request.Form["EmployeeDetails"];                    // 'EmployeeDetails' is coming from the form
            var userDtoObj = JsonConvert.DeserializeObject<UserDto>(userDtoString); // 'userDtotoString' is a string value which needs to be converted into an object of type <UserDto> using 'JsonConvert.DeserializeObject'
            var uploads = Path.Combine(_env.WebRootPath, "EmployeeImages");         // Combines the web root path (_env.WebRootPath) with a subdirectory named "EmployeeImages." This directory is likely where the uploaded image files will be stored on the server.


            if (userDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,                                        // because user is posting empty data, so must return bad request
                    Message = "Please send employee data to add!"
                });
            }

            foreach(var file in files)                         // In case we have to upload multiple images in the future
            {
                if(file.Length > 0)                            // It checks if the file's length is greater than 0 to ensure that it is not an empty file.
                {
                    if (!Directory.Exists(uploads))             // If the specified directory (referred to as "uploads") doesn't exist, it creates the directory.
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Split(";"); // This code will correctly retrieve the value of the "ASPNETCORE_ENVIRONMENT" environment variable and split it into an array of strings using semicolons as separators.

                    var filepath = Path.Combine(uploads, file.FileName);                        // This line constructs the full file path where the uploaded file will be saved. It combines the "uploads" directory path with the original file name (file.FileName) to create a complete file path.
                    using (var fileStream = new FileStream(filepath, FileMode.Create))          // This code creates a FileStream object for writing to the specified file path (filepath). It uses FileMode.Create a new file or overwrite an existing file if it already exists.
                    {
                        file.CopyTo(fileStream);                                                //  This line copies the contents of the uploaded file (file) to the fileStream, which is associated with the destination file. This effectively saves the uploaded file to the specified location on the server.
                    }
                    userDtoObj.ProfileImageUrl = $"EmployeeImages/{file.FileName}";             //  sets a property (userDtoObj.ProfileImageUrl) to a string representing the relative path to the uploaded file within the "EmployeeImages" directory.
                }
            }

            var userObj = _mapper.Map<UserModel>(userDtoObj);   //Convert Dto to Model and store in an object. We do the conversion becasue we need to access the updated properties to save in the database. We can't save as 'UserDto' in the database. Must convert it into 'Usermodel' and then save. 
            userObj.CreateDate = DateTime.Now;                   // Now we can access the properties and to update the 'CreateDate' & 'CreatedBy' properties
            userObj.CreatedBy = 3;                              // This value will be replaced by the 'ID' who creates data. This id gets changed by Angular
            EncDescPassword.CreateHashPassword(userDtoObj.Password, out byte[] passwordHash, out byte[] passwordSalt); //'userDtoObj.Password' passed in as parameter to encrypt it. 'EncDescPassword' can be directly accessed without creating an instance becasue its a static class
            userObj.PasswordHash = passwordHash; // Saving the encrypted 'passwordHash' from the previous line in the model instance 'userObj' to save in the database
            userObj.PasswordSalt = passwordSalt;
            Random r = new Random();
            int randonData = r.Next(1000,9999);               // Generate random number between 1000 & 9999
            userObj.UserName = userObj.FirstName+"."+userObj.LastName+randonData;
            await _userContext.UserModels.AddAsync(userObj);     // If there is some data in object 'userObj', add this in UserModels using instance '_userContext'..ie adding record in the database
            await _userContext.SaveChangesAsync();               // if added, I have to savechanges...i.e. saving the record in the database
            MailRequest mailRequest = new MailRequest();         // instance created to access the properties inside 'MailRequest' class
            mailRequest.ToEmail = userObj.Email;                 // to whom u want to send email
            mailRequest.Subject = "New User Created";
            mailRequest.Body = "Hi " + userObj.FirstName + " " + userObj.LastName + "," + "<br>" +                          // <br> break line.. next line
                                "<p>Your Profile has been created in our Company</p>"                                       // <p> paragraph tag
                                + "Below is your Login Credentials for accessing the Employee Portal<br>"
                                + "Your Email : " + "<strong>" + userDtoObj.Email + "</strong><br>"
                                + "<br>" +"Your Username : "+"<strong>"+userObj.UserName+"</strong><br>"
                                +"Your Password : " + "<strong>" + userDtoObj.Password +"</strong><br>"
                                             
                                +"Kindly Go on below link to Reset your password<br>" +
                                "<a href='https://localhost:44314/auth/reset'>Reset Password!</a>" +
                                "<br><br>Kind Regards,<br>Harish Nair";

            await _mailService.SendEmailAsync(mailRequest); // only accepts 'MailRequest' class object..ie 'mailRequest'
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Added!"
            });
        }

        // Updating the employee record
        // PUT api/<EmployeeController>/5
        [HttpPut("update")]
        //        public async Task<ActionResult<UserDto>> UpdateEmployee([FromBody] UserDto userDtoObj) // Get the data from 'form body' which is from UI, Postman or Angular; kind of data is 'UserDto'
        public async Task<ActionResult<UserDto>> UpdateEmployee() // Get the data from 'form body' which is from UI, Postman or Angular; kind of data is 'UserDto'
        {
            IFormFileCollection req = Request.Form.Files;                           // 'IFormFIleCollection' is an Interface provided by ASP.Net core Http, which collects your response of a file. If you receive any files in the http, it will collect it.
            var files = req;                                                        // The collected files are store inside var 'files'
            var userDtoString = Request.Form["EmployeeDetails"];                    // 'EmployeeDetails' is coming from the form
            var userDtoObj = JsonConvert.DeserializeObject<UserDto>(userDtoString); // 'userDtotoString' is a string value which needs to be converted into an object of type <UserDto> using 'JsonConvert.DeserializeObject'
            var uploads = Path.Combine(_env.WebRootPath, "EmployeeImages");

            if (userDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please send employee data to update!"
                });
            }
            var isUserExist = await _userContext.UserModels.AsNoTracking().FirstOrDefaultAsync(a => a.Id == userDtoObj.Id); // check if user exits; //do not track the same instance of the same entity  i.e. 'Id' & fetch the first usermodel that matches the given condition
            if (isUserExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found!"
                });
            }

            foreach (var file in files)                         // In case we have to upload multiple images in the future
            {
                if (file.Length > 0)                            // It checks if the file's length is greater than 0 to ensure that it is not an empty file.
                {
                    if (!Directory.Exists(uploads))             // If the specified directory (referred to as "uploads") doesn't exist, it creates the directory.
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Split(";"); // This code will correctly retrieve the value of the "ASPNETCORE_ENVIRONMENT" environment variable and split it into an array of strings using semicolons as separators.

                    var filepath = Path.Combine(uploads, file.FileName);                        // This line constructs the full file path where the uploaded file will be saved. It combines the "uploads" directory path with the original file name (file.FileName) to create a complete file path.
                    using (var fileStream = new FileStream(filepath, FileMode.Create))          // This code creates a FileStream object for writing to the specified file path (filepath). It uses FileMode.Create a new file or overwrite an existing file if it already exists.
                    {
                        file.CopyTo(fileStream);                                                //  This line copies the contents of the uploaded file (file) to the fileStream, which is associated with the destination file. This effectively saves the uploaded file to the specified location on the server.
                    }
                    userDtoObj.ProfileImageUrl = $"EmployeeImages/{file.FileName}";             //  sets a property (userDtoObj.ProfileImageUrl) to a string representing the relative path to the uploaded file within the "EmployeeImages" directory.
                }
            }   

            var userObj = _mapper.Map<UserModel>(userDtoObj); // Since the user exists, we need to convet the 'DTO object' to 'model object' inorder to save in the database 
            userObj.UpdateDate = DateTime.Now;                // Now we can access the properties and to update the 'UpdateDate' & 'UpdatedBy' properties
            userObj.UpdatedBy = 1;                            // This value will be replaced by the 'ID' who creates data. This id gets changed by Angular

            userObj.UserName=isUserExist.UserName;
            userObj.PasswordHash = isUserExist.PasswordHash; // Saving the encrypted 'passwordHash' in the model instance 'userObj' to save in the database
            userObj.PasswordSalt = isUserExist.PasswordSalt;

            _userContext.Entry(userObj).State = EntityState.Modified; // if there is an Id, Update it// In the 'userObj' if you have modified 'firstname' and 'salary', it will take the 'userObj', check the 'state' & the state will be modified. 
            await _userContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Updated!"
            });

        }

        // User will send 'id' of the employee that needs to be deleted. We need to take that 'id' & check in the database if the user exits and then remove that user. If the 'id' doesn't exits, 'return Not Found()'
        // DELETE api/<EmployeeController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDto>> DeleteEmployee(int id)
        {
            var user = await _userContext.UserModels.FindAsync(id);   //check if user exits
            if (user == null)                                        // If no user exits // If you send a string as id, the it will be a BadRequest(). In this case, if the user == null, it should return NotFound()
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Employee not found!"
                });
            }
            _userContext.UserModels.Remove(user);                  // Since user is present, remove
            await _userContext.SaveChangesAsync();                 // Save Changes
            return Ok(new
            {
                StatusCode = 200,
                Message = "Employee Deleted!"
            });
        }
    }
}

// The a in the lambda expression is a parameter that represents each element of the collection being queried (UserModels in this case). It's like a placeholder for each element that will be evaluated against the condition. {parameter} => {Expression / Logic}
//So, 'a => a.Id == userObj.Id' can be interpreted as "for each element a in the collection, check if a.Id is equal to userObj.Id."
// In other words, this lambda expression specifies the condition that the 'Id' property of the elements in the 'UserModels' collection should match the 'Id' property of the 'userObj' object being compared.

// AsNoTracking(): This method indicates that the retrieved entity i.e 'Id' should not be tracked by the EF Core change tracker. It improves performance when you don't intend to modify the entity. Only read it.
// FirstOrDefaultAsync(a => a.Id == userObj.Id): This part of the code specifies the query to fetch the first user model that matches the given condition. It uses a lambda expression to define the condition, comparing the 'Id' property of, each 'UserModel' object with that of 'userObj' object.
// Overall, this code snippet retrieves the first user from the "UserModels" table that has a matching ID with the userObj.Id. The query is performed asynchronously, and the result is assigned to the isUserExist variable.

