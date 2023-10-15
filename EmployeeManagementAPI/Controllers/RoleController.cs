using AutoMapper;
using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]                                           // Add this attribute on the dept controller. comes from 'Program.cs' Line 51
    public class RoleController : ControllerBase
    {
        private readonly EmployeeContext _roleContext;
        private readonly IMapper _mapper;
        public RoleController(EmployeeContext roleContext, IMapper mapper)
        {
            _roleContext = roleContext;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<RoleDto>> GetAllRoles()
        {
            var roles = await _roleContext.RoleModels.ToListAsync();
            var mappedRoles = _mapper.Map<List<RoleDto>>(roles);
            return Ok(new ResponseDto<RoleDto>
            {
                StatusCode=200,
                Message="Success!",
                Result=mappedRoles
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult<RoleDto>> AddRole()
        {
            
            var roleDtoString = Request.Form["RoleDetails"];
            var roleDtoObj = JsonConvert.DeserializeObject<RoleDto>(roleDtoString);

            if (roleDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please add details to Add role!"
                });
            }
            var roleObj = _mapper.Map<RoleModel>(roleDtoObj);
            roleObj.CreateDate = DateTime.Now;
            roleObj.CreatedBy = 1;
            await _roleContext.RoleModels.AddAsync(roleObj);
            await _roleContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Role Added!"
            });

        }
        [HttpPut("update")]
        public async Task<ActionResult<RoleDto>> UpdateRole()
        {
            
            var roleDtoString = Request.Form["RoleDetails"];
            var roleDtoObj = JsonConvert.DeserializeObject<RoleDto>(roleDtoString);

            if (roleDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode=400,
                    Message="Please add details to update!"
                });
            }
            var roleObj = _mapper.Map<RoleModel>(roleDtoObj);

            var isRoleExist = _roleContext.RoleModels.AsNoTracking().FirstOrDefault(x => x.RoleId == roleDtoObj.RoleId);
            if (isRoleExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Role doesn't exist!"
                });
            }
            roleObj.UpdateDate = DateTime.Now;
            roleObj.UpdatedBy = 1;
            _roleContext.Entry(roleObj).State = EntityState.Modified;
            await _roleContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Role Updated!"
            });

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleModel>> DeleteRole(int id)
        {
            var role = await _roleContext.RoleModels.FindAsync(id);
            if(role == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Role doesn't exist!"
                });
            }
            _roleContext.RoleModels.Remove(role);
            await _roleContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Role Deleted!"
            });
        }
    }
}
