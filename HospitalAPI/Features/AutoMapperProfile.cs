using AutoMapper;
using HospitalAPI.Models.DataModels;
using HospitalAPI.Models.DTOs;

namespace HospitalAPI
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

        }
    }
}
