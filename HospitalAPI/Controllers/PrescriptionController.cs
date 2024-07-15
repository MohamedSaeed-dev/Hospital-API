using HospitalAPI.Features.Pagination;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _servicePrescription;
        public PrescriptionController(IPrescriptionService servicePrescription)
        {
            _servicePrescription = servicePrescription;
        }
        // GET: api/<PrescriptionController>
        [HttpGet]
        [PaginationFilter]
        public async Task<IEnumerable<Prescription>> GetAllPrescriptions([FromQuery] PaginationQuery paginationQuery)
        {
            PaginationIndexes indexes = (PaginationIndexes)HttpContext.Items["PaginationIndexes"]!;
            return await _servicePrescription.GetAll(indexes.Skip, indexes.Take);
        }

        // GET api/<PrescriptionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var prescription = await _servicePrescription.GetById(id);
                if (prescription == null) return BadRequest(new { message = $"There is no prescription with Id={id}" });
                return Ok(new { prescription = prescription });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}" });
            }
        }

        // POST api/<PrescriptionController>
        [HttpPost]
        public async Task<IActionResult> AddAPrescription([FromBody] PrescriptionDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var isSuccessfull = await _servicePrescription.Add(entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to add the prescription" });
                return Ok(new { message = "The prescription is added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}" });
            }
        }

        // PUT api/<PrescriptionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var isSuccessfull = await _servicePrescription.Update(id, entity);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to update the prescription" });
                return Ok(new { message = "The prescription is updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}" });
            }
        }

        // DELETE api/<PrescriptionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var isSuccessfull = await _servicePrescription.DeleteById(id);
                if (isSuccessfull <= 0) return StatusCode(500, new { message = "Failed to delete the prescription" });
                return Ok(new { message = "The prescription is deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}" });
            }
        }
    }
}
