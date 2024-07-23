using Azure;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.Services;
using HospitalAPI.ServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers
{
    [Authorize(Roles = "Admin , Receptionist")]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _servicePatient;
        public PatientsController(IPatientService servicePatient)
        {
            _servicePatient = servicePatient;
        }
        // GET: api/<PatientController>
        [HttpGet]
        public async Task<PagedList<Patient>> GetAllPatient([FromQuery] GetAllQueries queries)
        {;
            return await _servicePatient.GetAll(queries);
        }

        // GET api/<PatientController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var patient = await _servicePatient.GetById(id);
                if (patient == null) return BadRequest(new { message = $"There is no patient with Id={id}" });
                return Ok(patient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<PatientController>
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] PatientDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var response = await _servicePatient.Add(entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<PatientController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var response =  await _servicePatient.Update(id, entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<PatientController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _servicePatient.DeleteById(id);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("Departments/{departmentId}")]
        public async Task<IActionResult> GetPatientsAtDepartment(int departmentId)
        {
            try
            {
                if (departmentId <= 0) return BadRequest(new { message = "Invalid Id" });
                return Ok(await _servicePatient.GetPatientsAtDepartment(departmentId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("Doctors/{doctorId}")]
        public async Task<IActionResult> GetPatientsAtDoctor(int doctorId)
        {
            try
            {
                if (doctorId <= 0) return BadRequest(new { message = "Invalid Id" });
                return Ok(await _servicePatient.GetPatientsAtDoctor(doctorId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("Appointments/{appointmentId}")]
        public async Task<IActionResult> GetPateintAtAppointment(int appointmentId)
        {
            try
            {
                if (appointmentId <= 0) return BadRequest(new { message = "Invalid Id" });
                return Ok(await _servicePatient.GetPateintAtAppointment(appointmentId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
