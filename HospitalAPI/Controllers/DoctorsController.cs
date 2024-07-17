using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _serviceDoctor;
        public DoctorsController(IDoctorService serviceDoctor)
        {
            _serviceDoctor = serviceDoctor;
        }
        // GET: api/<DoctorController>
        [HttpGet]
        [PaginationFilter]
        public async Task<IEnumerable<Doctor>> GetAllDoctors([FromQuery] PaginationQuery paginationQuery)
        {
            PaginationIndexes indexes = (PaginationIndexes)HttpContext.Items["PaginationIndexes"]!;
            return await _serviceDoctor.GetAll(indexes.Skip, indexes.Take);
        }

        // GET api/<DoctorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var doctor = await _serviceDoctor.GetById(id);
                if (doctor == null) return BadRequest(new { message = $"There is no doctor with Id={id}" });
                return Ok(new { doctor = doctor });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<DoctorController>
        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var isSuccessfull = await _serviceDoctor.Add(entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to add the doctor" });
                return Ok(new { message = "The doctor is added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<DoctorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var isSuccessfull = await _serviceDoctor.Update(id, entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to update the doctor" });
                return Ok(new { message = "The doctor is updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<DoctorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var isSuccessfull = await _serviceDoctor.DeleteById(id);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to delete the doctor" });
                return Ok(new { message = "The doctor is deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("Departments/{departmentId}")]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            try
            {
                if (departmentId <= 0) return BadRequest(new { message = "Invalid Id" });
                return Ok(new { doctors = await _serviceDoctor.GetDoctorsByDepartment(departmentId) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
