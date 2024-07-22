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
            CreateMap<Doctor, DoctorDTO>();

            CreateMap<DoctorDTO, Doctor>();

            CreateMap<PatientDTO, Patient>();

            CreateMap<Patient, PatientDTO>();

            CreateMap<Department, DepartmentDTO>();
            CreateMap<DepartmentDTO, Department>();

            CreateMap<Appointment, AppointmentDTO>();
            CreateMap<AppointmentDTO, Appointment>();

            CreateMap<Billing, BillingDTO>();
            CreateMap<BillingDTO, Billing>();

            CreateMap<Prescription, PrescriptionDTO>();
            CreateMap<PrescriptionDTO, Prescription>();

            CreateMap<Department, DepartmentDTO>();
            CreateMap<DepartmentDTO, Department>();

            CreateMap<Appointment, AppointmentDTO>();
            CreateMap<AppointmentDTO, Appointment>();

            CreateMap<Billing, BillingDTO>();
            CreateMap<BillingDTO, Billing>();

            CreateMap<Prescription, PrescriptionDTO>();
            CreateMap<PrescriptionDTO, Prescription>();

            CreateMap<User, UserSignUp>();
            CreateMap<UserSignUp, User>();

            CreateMap<User, UserLogin>();
            CreateMap<UserLogin, User>();

            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();

            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();

            /*CreateMap<IQueryable<User>, IQueryable<UserViewModel>>();
            CreateMap<IQueryable<UserViewModel>, IQueryable<User>>();*/

        }
    }
}
