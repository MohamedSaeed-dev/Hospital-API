using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IAppointmentService : IServiceAPI<Appointment, AppointmentDTO>
    {

    }
    public class AppointmentService : IAppointmentService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public AppointmentService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<int> Add(AppointmentDTO entity)
        {
            try
            {
                Appointment appointment = _mapper.Map<Appointment>(entity);
                await _db.Appointments.AddAsync(appointment);
                return _db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Appointments.FindAsync(Id);
                if (record == null) return 0;
                _db.Appointments.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetAll(int skip, int take)
        {
            return await _db.Appointments
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Appointment?> GetById(int Id)
        {
            return await _db.Appointments
                .SingleOrDefaultAsync(x => x.Id == Id); 
        }

        public async Task<int> Update(int Id, AppointmentDTO entity)
        {
            try
            {
                var record = await _db.Appointments.FindAsync(Id);
                if (record == null) return 0;

                if (entity.Status.HasValue) record.Status = entity.Status.Value;
                if (entity.DateTime.HasValue) record.DateTime = entity.DateTime.Value;
                if (entity.DoctorPatientId.HasValue) record.DoctorPatientId = entity.DoctorPatientId.Value;

                return await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
