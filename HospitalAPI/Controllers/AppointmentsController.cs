using HospitalAPI.Features;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalAPI.Controllers
{
    [Authorize(Roles = "Admin , Receptionist")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _serviceAppointment;
        public AppointmentsController(IAppointmentService serviceAppointment)
        {
            _serviceAppointment = serviceAppointment;
        }
        // GET: api/<AppointmentsController>
        [HttpGet]
        public async Task<PagedList<Appointment>> GetAllAppointments([FromQuery] GetAllQueries queries)
        {
            return await _serviceAppointment.GetAll(queries);
        }
        // GET api/<AppointmentsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var appointment = await _serviceAppointment.GetById(id);
                if (appointment == null) return BadRequest(new { message = $"There is no appointment with Id={id}" });
                return Ok(appointment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
        [HttpGet("startDate=&endDate=")]
        public async Task<IActionResult> AppointmentsAtDateRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                if (!startDate.HasValue || !endDate.HasValue) return BadRequest(new { message = "No Dates!" });
                var appointments = await _serviceAppointment.AppointmentsAtDateRange(startDate.Value, endDate.Value);
                if (appointments == null) return BadRequest(new { message = $"There is no appointments" });
                return Ok(new { appointments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<AppointmentsController>
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var response = await _serviceAppointment.Add(entity);
                return StatusCode(response.StatusCode, new { message = response.Message } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<AppointmentsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var response = await _serviceAppointment.Update(id, entity);
                return StatusCode(response.StatusCode, new { message = response.Message } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<AppointmentsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _serviceAppointment.DeleteById(id);
                return StatusCode(response.StatusCode, new { message = response.Message } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
