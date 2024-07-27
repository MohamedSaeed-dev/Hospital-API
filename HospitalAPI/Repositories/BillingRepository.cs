using AutoMapper;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalAPI.Repositories
{
    public class BillingRepository : IBillingService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public BillingRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }

        public async Task<ResponseStatus> Add(BillingDTO entity)
        {
            try
            {
                Billing billing = _mapper.Map<Billing>(entity);
                await _db.Billings.AddAsync(billing);
                await _db.SaveChangesAsync();
                return _response.Created("Bill is Created Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseStatus> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Billings.FindAsync(Id);
                if (record == null) return _response.BadRequest("Bill is not exist");
                _db.Billings.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Bill is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Billing>> GetAll(GetAllQueries queries)
        {
            var billings = queries.SortOrder.ToLower() == "desc" ?
                _db.Billings.OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Billings.OrderBy(GetProperty(queries.SortColumn));

            return await PagedList<Billing>.CreatePagedList(_db.Billings, queries.Page, queries.PageSize);
        }

        public Expression<Func<Billing, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "status" => d => d.Status!,
                "date" => d => d.DateTime!,
                _ => d => d.AppointmentId
            };
        }

        public async Task<Billing?> GetById(int Id)
        {
            return await _db.Billings.SingleOrDefaultAsync(x => x.AppointmentId == Id);
        }

        public async Task<ResponseStatus> Update(int Id, BillingDTO entity)
        {
            try
            {
                var record = await _db.Billings.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return _response.BadRequest("Bill is not exist");

                if (entity.Status.HasValue) record.Status = entity.Status.Value;
                if (entity.Amount.HasValue) record.Amount = entity.Amount.Value;
                if (entity.AppointmentId.HasValue) record.AppointmentId = entity.AppointmentId.Value;

                await _db.SaveChangesAsync();
                return _response.Ok("Bill is Updated Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
