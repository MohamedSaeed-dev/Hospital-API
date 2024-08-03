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
    public class DoctorRepository : IDoctorService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public DoctorRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }
        public async Task<ResponseStatus> Add(DoctorDTO entity)
        {
            try
            {
                var isExist = _db.Doctors.Any(x => x.FullName == entity.FullName);
                if (isExist) return _response.BadRequest("Doctor is already exist");
                Doctor doctor = _mapper.Map<Doctor>(entity);
                await _db.Doctors.AddAsync(doctor);
                await _db.SaveChangesAsync();
                return _response.Created("Doctor is Created Successfully");
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
                var record = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Doctor is not exist");
                _db.Doctors.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Doctor is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Doctor>> GetAll(GetAllQueries queries)
        {
            var doctors = queries.SortOrder.ToLower() == "desc" ?
                _db.Doctors.Where(x => x.FullName!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Doctors.Where(x => x.FullName!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderBy(GetProperty(queries.SortColumn));

            return await PagedList<Doctor>.CreatePagedList(doctors, queries.Page, queries.PageSize);
        }

        public Expression<Func<Doctor, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "name" => d => d.FullName! ,
                "gender" => d => d.Gender!,
                "salary" => d => d.Salary,
                _ => d => d.Id
            };
        }

        public async Task<Doctor?> GetById(int Id)
        {
            return await _db.Doctors
                .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByDepartment(int departmentId)
        {
            return await _db.Doctors.Where(x => x.Department.Id == departmentId)
                .ToListAsync();
        }

        public async Task<ResponseStatus> Update(int Id, DoctorDTO entity)
        {
            try
            {
                var record = await _db.Doctors.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Doctor is not exist");

                if (!string.IsNullOrEmpty(entity.FullName)) record.FullName = entity.FullName;
                if (!string.IsNullOrEmpty(entity.Email)) record.Email = entity.Email;
                if (!string.IsNullOrEmpty(entity.Phone)) record.Phone = entity.Phone;
                if (!string.IsNullOrEmpty(entity.Address)) record.Address = entity.Address;

                if (entity.Gender.HasValue) record.Gender = entity.Gender;
                if (entity.DepartmentId.HasValue) record.DepartmentId = entity.DepartmentId.Value;
                if (entity.Salary.HasValue) record.Salary = entity.Salary.Value;
                if (entity.Status.HasValue) record.Status = entity.Status.Value;
                if (entity.BonusRate.HasValue) record.BonusRate = entity.BonusRate.Value;

                await _db.SaveChangesAsync();
                return _response.Ok("Doctor is Updated Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
