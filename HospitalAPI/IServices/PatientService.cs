using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DbContextModel;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;
using HospitalAPI.ServicesAPI;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public interface IPatientService : IServiceAPI<Patient, PatientDTO>
    {
        Task<IEnumerable<Patient>> GetPatientsAtDoctor(int doctorId);
        Task<IEnumerable<Patient>> GetPatientsAtDepartment(int departmentId);
        Task<PateintAppointmentViewModel?> GetPateintAtAppointment(int appointmentId);
    }
}
