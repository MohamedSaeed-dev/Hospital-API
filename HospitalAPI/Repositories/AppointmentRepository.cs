using AutoMapper;
using HospitalAPI.Features;
using HospitalAPI.Features.Pagination;
using HospitalAPI.Features.Utils.IServices;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalAPI.Repositories
{
    public class AppointmentRepository : IAppointmentService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public AppointmentRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }
        public async Task<ResponseStatus> Add(AppointmentDTO entity)
        {
            try
            {
                Appointment appointment = _mapper.Map<Appointment>(entity);
                await _db.Appointments.AddAsync(appointment);
                await _db.SaveChangesAsync();
                return _response.Created("Appointment is Created Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> AppointmentsAtDateRange(DateTime startDate, DateTime endDate)
        {
            return await _db.Appointments.Where(x => x.DateTime >= startDate && x.DateTime <= endDate).ToListAsync();
        }

        public async Task<ResponseStatus> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Appointments.FindAsync(Id);
                if (record == null) return _response.BadRequest("Appointment is not exist");
                _db.Appointments.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Appointment is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Appointment>> GetAll(GetAllQueries queries)
        {
            var appointments = queries.SortOrder.ToLower() == "desc" ?
                _db.Appointments.OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Appointments.OrderBy(GetProperty(queries.SortColumn));
            return await PagedList<Appointment>.CreatePagedList(appointments, queries.Page, queries.PageSize);
        }

        public Expression<Func<Appointment, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "status" => d => d.Status!,
                "date" => d => d.DateTime!,
                _ => d => d.Id
            };
        }

        public async Task<Appointment?> GetById(int Id)
        {
            return await _db.Appointments
                .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<ResponseStatus> Update(int Id, AppointmentDTO entity)
        {
            try
            {
                var record = await _db.Appointments.FindAsync(Id);
                if (record == null) return _response.BadRequest("Appointment is not exist");

                if (entity.Status.HasValue) record.Status = entity.Status.Value;
                if (entity.DateTime.HasValue) record.DateTime = entity.DateTime.Value;
                if (entity.DoctorPatientId.HasValue) record.DoctorPatientId = entity.DoctorPatientId.Value;

                await _db.SaveChangesAsync();
                return _response.Ok("Appointment is Updated Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
