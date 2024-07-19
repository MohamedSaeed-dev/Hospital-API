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
        Task<IEnumerable<Doctor>> GetDoctorsByDepartment(int departmentId);
    }
}
