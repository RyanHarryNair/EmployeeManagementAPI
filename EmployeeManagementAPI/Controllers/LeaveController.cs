using AutoMapper;
using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly EmployeeContext _leaveContext;
        private readonly IMapper _mapper;
        public LeaveController(EmployeeContext context, IMapper mapper)
        {
            _mapper = mapper;
            _leaveContext = context;
        }
        [HttpGet]
        public async Task<ActionResult<LeaveDto>> GetAllLeaves()         
        {
            var leaves = await (from leave in _leaveContext.LeaveModels             // using Entity Framework Core's LINQ query syntax to retrieve leave information from the database and project it into a list of LeaveDto objects. Firing SQL qurry in .NET Core
                                join user in _leaveContext.UserModels on
                          leave.UserId equals user.Id
                          select new LeaveDto
                          {
                              UserId = leave.UserId,
                              UserName = user.FirstName + " " + user.LastName,
                              LStartDate = leave.LStartDate,
                              LEndDate = leave.LEndDate,
                              LeaveType = leave.LeaveType,
                              Count = leave.Count,
                              Status = leave.Status,
                              LeaveId = leave.LeaveId,
                          }).ToListAsync();
            var _mappedLeaves = _mapper.Map<List<LeaveDto>>(leaves);
            return Ok(new ResponseDto<LeaveDto>
            {
                StatusCode=200,
                Message="Success",
                Result= _mappedLeaves
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult<LeaveDto>> AddLeave()
        {
            IFormFileCollection req = Request.Form.Files;
            var leaveDtoString = Request.Form["LeaveDetails"];
            var leaveDtoObj = JsonConvert.DeserializeObject<LeaveDto>(leaveDtoString);

            if (leaveDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please add leave details!",
                });
            }
            var leaveObj = _mapper.Map<LeaveModel>(leaveDtoObj);
            leaveObj.Status = "Pending";
            //leaveObj.CreateDate = DateTime.Now;
            //leaveObj.CreatedBy = 1;
            await _leaveContext.LeaveModels.AddAsync(leaveObj);
            await _leaveContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Leave Added!",
            });
        }
        [HttpPut("update")]
        public async Task<ActionResult<LeaveDto>> UpdateLeave()
        {
            IFormFileCollection req = Request.Form.Files;
            var leaveDtoString = Request.Form["LeaveDetails"];
            var leaveDtoObj = JsonConvert.DeserializeObject<LeaveDto>(leaveDtoString);

            if (leaveDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please add leave details to update!",
                });
            }
            var leaveObj = _mapper.Map<LeaveModel>(leaveDtoObj);

            var isLeaveExist = _leaveContext.LeaveModels.AsNoTracking().FirstOrDefault(x => x.LeaveId == leaveDtoObj.LeaveId);
            if (isLeaveExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Leave not found!",
                });
            }
            leaveObj.UpdateDate = DateTime.Now;
            leaveObj.UpdatedBy = 1;
            _leaveContext.Entry(leaveObj).State = EntityState.Modified;
            await _leaveContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Leave Updated!",
            });
        }
        [HttpGet("userId")]
        public async Task<ActionResult<LeaveDto>> GetLeavesByUserId(int userId)
        {
            var isExist = await _leaveContext.LeaveModels.Where(a => a.UserId == userId).FirstOrDefaultAsync();
            if (isExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Leave not found for user!",
                });
            }
            var myLeaves = await _leaveContext.LeaveModels.Where(a => a.UserId == userId).ToListAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Success!",
                Result=myLeaves
            });
        }
    }
}
