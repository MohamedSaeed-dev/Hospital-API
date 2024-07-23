using Azure;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using HospitalAPI.ServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HospitalAPI.Controllers
{
    [Authorize(Roles = "Admin , Receptionist")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _serviceDepartment;
        public DepartmentsController(IDepartmentService serviceDepartment)
        {
            _serviceDepartment = serviceDepartment;
        }
        // GET: api/<DepartmentController>
        [HttpGet]
        public async Task<PagedList<Department>> GetAllDepartments([FromQuery] GetAllQueries queries)
        {
            return await _serviceDepartment.GetAll(queries);
        }

        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var department = await _serviceDepartment.GetById(id);
                if (department == null) return BadRequest(new { message = $"There is no department with Id={id}"});
                return Ok(department);
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDTO entity)
        {
            try
            {
                if(entity == null) return BadRequest(new { message = "No Data provided!" });
                var response = await _serviceDepartment.Add(entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var response = await _serviceDepartment.Update(id, entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _serviceDepartment.DeleteById(id);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
