using Azure;
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
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _serviceBilling;
        public BillingController(IBillingService serviceBilling)
        {
            _serviceBilling = serviceBilling;
        }
        // GET: api/<BillingController>
        [HttpGet]
        public async Task<PagedList<Billing>> GetAllBillings([FromQuery] GetAllQueries queries)
        {
            return await _serviceBilling.GetAll(queries);
        }

        // GET api/<BillingController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillingById(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var billing = await _serviceBilling.GetById(id);
                if (billing == null) return BadRequest(new { message = $"There is no billing with Id={id}" });
                return Ok(billing);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // POST api/<BillingController>
        [HttpPost]
        public async Task<IActionResult> AddBilling([FromBody] BillingDTO entity)
        {
            try
            {
                if (entity == null) return BadRequest(new { message = "No Data provided!" });
                var response = await _serviceBilling.Add(entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // PUT api/<BillingController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBilling(int id, [FromBody] BillingDTO entity)
        {
            try
            {
                if (id <= 0 || entity == null) return BadRequest(new { message = "Invalid entered data" });
                var response = await _serviceBilling.Update(id, entity);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }

        // DELETE api/<BillingController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int id)
        {
            try
            {
                if (id <= 0) return BadRequest(new { message = "Invalid Id" });
                var response = await _serviceBilling.DeleteById(id);
                return StatusCode(response.StatusCode, new { message = response.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", Error = $"{ex.Message}", InnerError = $"{ex.InnerException?.Message}" });
            }
        }
    }
}
