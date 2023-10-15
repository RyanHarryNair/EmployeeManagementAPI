using AutoMapper;
using EmployeeManagementAPI.Data.Context;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EmployeeManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]                                               // Add this attribute on the dept controller. comes from 'Program.cs' Line 51
    public class DepartmentController : ControllerBase
    {
        private readonly EmployeeContext _deptContext;
        private readonly IMapper _mapper;
        public DepartmentController(EmployeeContext deptContext, IMapper mapper)
        {
            _deptContext = deptContext;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<DepartmentDto>> GetAllDepartsMents()
        {
            var departmentList = await _deptContext.DepartmentModels.ToListAsync();
            var _mappedDept = _mapper.Map<List<DepartmentDto>>(departmentList);            // because we need to show onfo to user
            return Ok(new ResponseDto<DepartmentDto>
            {
                StatusCode = 200,
                Message = "Success",
                Result = _mappedDept
            });
        }
        [HttpPost("add")]
        public async Task<ActionResult<DepartmentDto>> AddDepartMent()
        {
            
            var deptDtoString = Request.Form["DepartmentDetails"];
            var deptDtoObj = JsonConvert.DeserializeObject<DepartmentDto>(deptDtoString);

            if (deptDtoObj == null)
            {
                return BadRequest(new ResponseDto<DepartmentDto>
                {
                    StatusCode=400,
                    Message = "Please send data to add!"
                });
            }
            var _mappedDept = _mapper.Map<DepartmentModel>(deptDtoObj); // converted dto to model to store in db
           // _mappedDept.CreateDate = DateTime.Now;
           // _mappedDept.CreatedBy = 1;
            _deptContext.DepartmentModels.Add(_mappedDept);
            await _deptContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Department Added"
            });
        }
        [HttpPut("update")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment()
        {
                                                                                              
            var deptDtoString = Request.Form["DepartmentDetails"];                   
            var deptDtoObj = JsonConvert.DeserializeObject<DepartmentDto>(deptDtoString);


            if (deptDtoObj == null)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Please send data to update!"
                });
            }
            var isDeptExist = await _deptContext.DepartmentModels.AsNoTracking().FirstOrDefaultAsync(a => a.DeptId == deptDtoObj.DeptId);
            if (isDeptExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Department not found!"
                });
            }
            var deptObj = _mapper.Map<DepartmentModel>(deptDtoObj);
            //deptObj.UpdateDate = DateTime.Now;
            //deptObj.UpdatedBy = 1;
            _deptContext.Entry(deptObj).State = EntityState.Modified;
            await _deptContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Department Updated"
            });

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<DepartmentDto>> DeleteDepartment(int id)
        {
            var department = await _deptContext.DepartmentModels.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            _deptContext.DepartmentModels.Remove(department);
            await _deptContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 404,
                Message = "Department Deleted!"
            });
        }
    }
}
