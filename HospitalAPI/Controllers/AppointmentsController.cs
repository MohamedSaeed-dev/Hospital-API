using HospitalAPI.Features;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalAPI.Controllers
{
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
        [PaginationFilter]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IEnumerable<Appointment>> GetAllAppointments([FromQuery] PaginationQuery paginationQuery)
        {
            PaginationIndexes indexes = (PaginationIndexes)HttpContext.Items["PaginationIndexes"]!;
            return await _serviceAppointment.GetAll(indexes.Skip, indexes.Take);
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
                return Ok(new { appointment = appointment });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
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
                return Ok(new { appointment = appointments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<AppointmentsController>
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var isSuccessfull = await _serviceAppointment.Add(entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to add the appointment" });
                return Ok(new { message = "The appointment is added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<AppointmentsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var isSuccessfull = await _serviceAppointment.Update(id, entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to update the appointment" });
                return Ok(new { message = "The appointment is updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<AppointmentsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var isSuccessfull = await _serviceAppointment.DeleteById(id);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to delete the appointment" });
                return Ok(new { message = "The appointment is deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", Inner = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
