using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using HospitalAPI.ServicesAPI;
using Microsoft.AspNetCore.Mvc;


namespace HospitalAPI.Controllers
{
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
        [PaginationFilter]
        public async Task<IEnumerable<Department>> GetAllDepartments([FromQuery] PaginationQuery paginationQuery)
        {
            PaginationIndexes indexes = (PaginationIndexes)HttpContext.Items["PaginationIndexes"]!;
            return await _serviceDepartment.GetAll(indexes.Skip, indexes.Take);
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
                return Ok(new { department = department });
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDTO entity)
        {
            try
            {
                if(entity == null) return BadRequest(new { message = "No Data provided!" });
                var isSuccessfull = await _serviceDepartment.Add(entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to add the department" });
                return Ok(new { message = "The department is added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var isSuccessfull = await _serviceDepartment.Update(id, entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to update the department" });
                return Ok(new { message = "The department is updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var isSuccessfull = await _serviceDepartment.DeleteById(id);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to delete the department" });
                return Ok(new { message = "The department is deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
