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
    public class PrescriptionRepository : IPrescriptionService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public PrescriptionRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }

        public async Task<ResponseStatus> Add(PrescriptionDTO entity)
        {
            try
            {
                Prescription prescription = _mapper.Map<Prescription>(entity);
                await _db.Prescriptions.AddAsync(prescription);
                await _db.SaveChangesAsync();
                return _response.Created("Prescription is Created Successfully");

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
                var record = await _db.Prescriptions.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return _response.BadRequest("Prescription is not exist");
                _db.Prescriptions.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Prescription is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Prescription>> GetAll(GetAllQueries queries)
        {
            var prescriptions = queries.SortOrder.ToLower() == "desc" ?
                _db.Prescriptions.Where(x => x.Medication!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Prescriptions.Where(x => x.Medication!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderBy(GetProperty(queries.SortColumn));

            return await PagedList<Prescription>.CreatePagedList(_db.Prescriptions, queries.Page, queries.PageSize);
        }

        public Expression<Func<Prescription, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "startdate" => d => d.StartDate!,
                "enddate" => d => d.EndDate!,
                "medication" => d => d.Medication,
                _ => d => d.AppointmentId
            };
        }

        public async Task<Prescription?> GetById(int Id)
        {

            return await _db.Prescriptions
                .SingleOrDefaultAsync(x => x.AppointmentId == Id);

        }

        public async Task<ResponseStatus> Update(int Id, PrescriptionDTO entity)
        {
            try
            {
                var record = await _db.Prescriptions.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return _response.BadRequest("Prescription is not exist");

                if (!string.IsNullOrEmpty(entity.Medication)) record.Medication = entity.Medication;

                if (entity.StartDate.HasValue) record.StartDate = entity.StartDate.Value;
                if (entity.EndDate.HasValue) record.EndDate = entity.EndDate.Value;
                if (entity.AppointmentId.HasValue) record.AppointmentId = entity.AppointmentId.Value;

                await _db.SaveChangesAsync();
                return _response.Ok("Prescription is Updated Suucessfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
