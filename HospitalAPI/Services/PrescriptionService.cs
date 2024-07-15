using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IPrescriptionService : IServiceAPI<Prescription, PrescriptionDTO>
    {

    }
    public class PrescriptionService : IPrescriptionService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public PrescriptionService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> Add(PrescriptionDTO entity)
        {
            try
            {
                Prescription prescription = _mapper.Map<Prescription>(entity);
                await _db.Prescriptions.AddAsync(prescription);
                return _db.SaveChanges();

            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> DeleteById(int Id)
        {
            try
            {
                var record = await _db.Prescriptions.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return 0;
                _db.Prescriptions.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Prescription>> GetAll(int? skip, int? take)
        {
            return await _db.Prescriptions.Include(x => x.Appointment).Skip((int)skip).Take((int)take).ToListAsync();
        }

        public async Task<Prescription?> GetById(int Id)
        {

            return await _db.Prescriptions.Include(x => x.Appointment).SingleOrDefaultAsync(x => x.AppointmentId == Id);

        }

        public async Task<int> Update(int Id, PrescriptionDTO entity)
        {
            try
            {
                var record = await _db.Prescriptions.SingleOrDefaultAsync(x => x.AppointmentId == Id);
                if (record == null) return 0;

                if(entity.StartDate.HasValue) record.StartDate = entity.StartDate.Value;
                if(entity.EndDate.HasValue) record.EndDate = entity.EndDate.Value;
                if(entity.AppointmentId.HasValue) record.AppointmentId = entity.AppointmentId.Value;
                if(entity.Medication != null) record.Medication = entity.Medication;

                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
