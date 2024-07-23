using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;
using HospitalAPI.Models.ViewModels;

namespace HospitalAPI.Features.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Doctor, DoctorDTO>().ReverseMap();

            CreateMap<PatientDTO, Patient>().ReverseMap();

            CreateMap<Department, DepartmentDTO>().ReverseMap();

            CreateMap<Appointment, AppointmentDTO>().ReverseMap();

            CreateMap<Billing, BillingDTO>().ReverseMap();

            CreateMap<Prescription, PrescriptionDTO>().ReverseMap();

            CreateMap<Department, DepartmentDTO>().ReverseMap();

            CreateMap<Appointment, AppointmentDTO>().ReverseMap();

            CreateMap<Billing, BillingDTO>().ReverseMap();

            CreateMap<Prescription, PrescriptionDTO>().ReverseMap();

            CreateMap<User, UserSignUp>().ReverseMap();

            CreateMap<User, UserLogin>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
