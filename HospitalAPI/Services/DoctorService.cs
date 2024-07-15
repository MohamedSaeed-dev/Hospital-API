using AutoMapper;
using HospitalAPI.Models;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IDoctorService : IServiceAPI<Doctor, DoctorDTO>
    {
        Task<IEnumerable<Doctor>> GetDoctorsByDepartment(int  departmentId);
    }
    public class DoctorService : IDoctorService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public DoctorService(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<int> Add(DoctorDTO entity)
        {
            try
            {
                Doctor doctor = _mapper.Map<Doctor>(entity);
                await _db.Doctors.AddAsync(doctor);
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
                var record = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;
                _db.Doctors.Remove(record);
                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Doctor>> GetAll(int? skip, int? take)
        {
            return await _db.Doctors
                .Select(p => new Doctor
                {
                    Address = p.Address,
                    BonusRate = p.BonusRate,
                    Department = p.Department,
                    Email = p.Email,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    Id = p.Id,
                    Phone = p.Phone,
                    Salary = p.Salary,
                    Status = p.Status,
                    Appointments = p.Appointments,

                }).Skip((int)skip).Take((int)take)
                .ToListAsync();

        }

        public async Task<Doctor?> GetById(int Id)
        {
            return await _db.Doctors
                .Select(p => new Doctor
                {
                    Address = p.Address,
                    BonusRate = p.BonusRate,
                    Department = p.Department,
                    Email = p.Email,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    Id = p.Id,
                    Phone = p.Phone,
                    Salary = p.Salary,
                    Status = p.Status,
                    Appointments= p.Appointments,

                })
                .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByDepartment(int departmentId)
        { 
            return await _db.Doctors.Where(x => x.Department.Id == departmentId)
                .Select(x => new Doctor
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Gender = x.Gender,
                    Phone = x.Phone,
                }).ToListAsync();
        }

        public async Task<int> Update(int Id, DoctorDTO entity)
        {
            try
            {
                var record = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return 0;

                if (!string.IsNullOrEmpty(entity.FullName)) record.FullName = entity.FullName;
                if (!string.IsNullOrEmpty(entity.Email)) record.Email = entity.Email;
                if (!string.IsNullOrEmpty(entity.Phone)) record.Phone = entity.Phone;
                if (!string.IsNullOrEmpty(entity.Address)) record.Address = entity.Address;

                if (entity.Gender.HasValue) record.Gender = entity.Gender;
                if (entity.DepartmentId.HasValue) record.DepartmentId = entity.DepartmentId.Value;
                if (entity.Salary.HasValue) record.Salary = entity.Salary.Value;
                if (entity.Status.HasValue) record.Status = entity.Status.Value;
                if (entity.BonusRate.HasValue) record.BonusRate = entity.BonusRate.Value;

                return await _db.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}
