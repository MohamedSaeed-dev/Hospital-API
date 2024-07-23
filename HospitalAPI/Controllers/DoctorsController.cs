using Azure;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Authorize(Roles = "Admin , Receptionist")]
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
        public async Task<PagedList<Doctor>> GetAllDoctors([FromQuery] GetAllQueries queries)
        {;
            return await _serviceDoctor.GetAll(queries);
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
                return Ok(doctor);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<DoctorController>
        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var response = await _serviceDoctor.Add(entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<DoctorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var response = await _serviceDoctor.Update(id, entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<DoctorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _serviceDoctor.DeleteById(id);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
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
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
