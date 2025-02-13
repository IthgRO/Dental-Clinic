using AutoMapper;
using Dental_Clinic.Dtos;
using Dental_Clinic.Requests.Service;
using Dental_Clinic.Requests.User;
using Dental_Clinic.Responses.Appointment;
using Dental_Clinic.Responses.Clinic;
using Dental_Clinic.Responses.Dentist;
using Dental_Clinic.Responses.Service;
using Dental_Clinic.Responses.User;
using Services.Models.Clinic;
using Services.Models.Reservation;
using Services.Models.Service;
using Services.Models.User;

namespace Dental_Clinic.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region User

            CreateMap<RegisterUserRequest, RegisterUserDto>()
                    .ReverseMap();

            CreateMap<LoginUserRequest, LoginUserDto>()
                .ReverseMap();

            CreateMap<LoginResultDto, LoginResponse>()
                .ReverseMap();

            #endregion

            #region Clinic

            CreateMap<ClinicDto, ClinicViewModel>()
                .ReverseMap();

            #endregion

            #region Service

            CreateMap<Services.Models.Service.ServiceDto, ServiceViewModel>()
                .ReverseMap();

            CreateMap<AddServiceToDentistRequest, AvailableServiceViewModel>()
                .ReverseMap();

            CreateMap<AddServiceToDentistRequest, Services.Models.Service.ServiceDto>()
                .ReverseMap();

            CreateMap<DentistServiceViewModel, Services.Models.Service.ServiceDto>()
                .ReverseMap();

            CreateMap<ServiceViewModel, Dtos.ServiceDto>()
                .ReverseMap();

            #endregion


            #region Dentist

            CreateMap<DentistListDto, AvailableDentistsResponse>()
                .ReverseMap();

            CreateMap<UserDto, DentistViewModel>()
                .ForMember(x => x.MaximumPrice, opt => opt.MapFrom(x =>
                    x.Services.Select(s => s.Price).DefaultIfEmpty(0).Max()))
                .ForMember(x => x.MinimumPrice, opt => opt.MapFrom(x =>
                    x.Services.Select(s => s.Price).DefaultIfEmpty(0).Min()))
                .ReverseMap();

            #endregion

            #region Appointment

            CreateMap<Services.Models.Reservation.AppointmentDto, AppointmentViewModel>()
                .ReverseMap();

            CreateMap<DentistAppointmentDto, DentistAppointmentViewModel>()
                .ReverseMap();
            #endregion

            #region Clinic

            CreateMap<Clinic, DentistClinicViewModel>()
                .ReverseMap();

            #endregion


        }
    }
}
