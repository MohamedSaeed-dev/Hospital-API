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
        Task<IEnumerable<Appointment>> AppointmentsAtDateRange(DateTime startDate, DateTime endDate);
    }   
}
