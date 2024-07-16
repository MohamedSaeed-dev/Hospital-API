using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IPatientService : IServiceAPI<Patient, PatientDTO>
    {
        Task<IEnumerable<Patient>> GetPatientsAtDoctor(int doctorId);
        Task<IEnumerable<Patient>> GetPatientsAtDepartment(int departmentId);
    }
    public class PatientService : IPatientService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public PatientService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> Add(PatientDTO entity)
        {
            try
            {
                
                Patient patient = _mapper.Map<Patient>(entity);
                
                await _db.Patients.AddAsync(patient);
                await _db.SaveChangesAsync();

                var doctorPatient = new DoctorPatient
                {
                    DoctorId = entity.DoctorId,
                    PatientId = patient.Id
                };
                await _db.DoctorPatients.AddAsync(doctorPatient);
                return await _db.SaveChangesAsync();
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
                var record = await _db.Patients.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;
                _db.Patients.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Patient>> GetAll(int skip, int take)
        {
            return await _db.Patients
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task <Patient?> GetById(int Id)
        {

            return await _db.Patients
                .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsAtDepartment(int departmentId)
        {
            return await _db.Patients.Where(p => p.DoctorPatients.Any(dp => dp.Doctor.DepartmentId == departmentId))
            .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsAtDoctor(int doctorId)
        {
            return await _db.Patients.Where(p => p.DoctorPatients.Any(x => x.Doctor.Id == doctorId))
                .ToListAsync();
        }

        public async Task<int> Update(int Id, PatientDTO entity)
        {
            try
            {
                var record = await _db.Patients.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;

                if (!string.IsNullOrEmpty(entity.FullName)) record.FullName = entity.FullName;
                if (!string.IsNullOrEmpty(entity.Email)) record.Email = entity.Email;
                if (!string.IsNullOrEmpty(entity.Phone)) record.Phone = entity.Phone;
                if (!string.IsNullOrEmpty(entity.Address)) record.Address = entity.Address;
                if (entity.Gender.HasValue) record.Gender = entity.Gender;

                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
