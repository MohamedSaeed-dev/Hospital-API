using AutoMapper;
using HospitalAPI.Features;
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
    public class PatientRepository : IPatientService
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IResponseStatus _response;

        public PatientRepository(MyDbContext db, IMapper mapper, IResponseStatus response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }

        public async Task<ResponseStatus> Add(PatientDTO entity)
        {
            try
            {
                var isExist = _db.Patients.Any(x => x.FullName == entity.FullName);
                if (isExist) return _response.BadRequest("Patient is already exist");
                Patient patient = _mapper.Map<Patient>(entity);
                using(var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        await _db.Patients.AddAsync(patient);
                        await _db.SaveChangesAsync();
                        var doctorPatient = new DoctorPatient
                        {
                            DoctorId = entity.DoctorId,
                            PatientId = patient.Id
                        };
                        await _db.DoctorPatients.AddAsync(doctorPatient);
                        await _db.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                return _response.Created("Patient is Created Successfully");
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
                var record = await _db.Patients.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Patient is not exits");
                _db.Patients.Remove(record);
                await _db.SaveChangesAsync();
                return _response.Ok("Patient is Deleted Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<Patient>> GetAll(GetAllQueries queries)
        {
            var patients = queries.SortOrder.ToLower() == "desc" ?
                _db.Patients.Where(x => x.FullName!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderByDescending(GetProperty(queries.SortColumn)) :
                _db.Patients.Where(x => x.FullName!.ToLower().Contains(queries.SearchTerm.ToLower())).OrderBy(GetProperty(queries.SortColumn));

            return await PagedList<Patient>.CreatePagedList(patients, queries.Page, queries.PageSize);
        }

        public Expression<Func<Patient, object>> GetProperty(string sortColumn)
        {
            return sortColumn?.ToLower() switch
            {
                "name" => d => d.FullName!,
                "gender" => d => d.Gender!,
                _ => d => d.Id
            };
        }

        public async Task<Patient?> GetById(int Id)
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
        public async Task<PateintAppointmentViewModel?> GetPateintAtAppointment(int appointmentId)
        {
            return await (from DP in _db.DoctorPatients
                    join P in _db.Patients
                    on DP.PatientId equals P.Id
                    join D in _db.Doctors
                    on DP.DoctorId equals D.Id
                    join Dept in _db.Departments
                    on D.DepartmentId equals Dept.Id
                    join A in _db.Appointments
                    on DP.Id equals A.DoctorPatientId
                    join B in _db.Billings
                    on A.Id equals B.AppointmentId
                    where A.Id == appointmentId
                    select new PateintAppointmentViewModel
                    {
                        PatientName = P.FullName,
                        PatientGender = P.Gender.Value,
                        DoctorName = D.FullName,
                        DoctorDepartment = D.Department.Name,
                        DoctorGender = D.Gender.Value,
                        AppointmentDate = A.DateTime,
                        BillingAmount = B.Amount,
                        BillingStatus = B.Status
                    }).FirstOrDefaultAsync();
        }
        public async Task<ResponseStatus> Update(int Id, PatientDTO entity)
        {
            try
            {
                var record = await _db.Patients.SingleOrDefaultAsync(x => x.Id == Id);
                if (record == null) return _response.BadRequest("Patient is not exist");

                if (!string.IsNullOrEmpty(entity.FullName)) record.FullName = entity.FullName;
                if (!string.IsNullOrEmpty(entity.Email)) record.Email = entity.Email;
                if (!string.IsNullOrEmpty(entity.Phone)) record.Phone = entity.Phone;
                if (!string.IsNullOrEmpty(entity.Address)) record.Address = entity.Address;
                if (entity.Gender.HasValue) record.Gender = entity.Gender;

                await _db.SaveChangesAsync();
                return _response.Ok("Patient is Upadated Succesffully");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
