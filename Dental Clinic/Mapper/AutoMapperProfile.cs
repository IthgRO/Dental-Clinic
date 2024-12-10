using AutoMapper;
using Dental_Clinic.Dtos;
using Dental_Clinic.Requests.User;
using Dental_Clinic.Responses.Appointment;
using Dental_Clinic.Responses.Clinic;
using Dental_Clinic.Responses.Dentist;
using Dental_Clinic.Responses.Service;
using Services.Models.Reservation;
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

            #endregion

            #region Clinic

            CreateMap<ClinicDto, ClinicViewModel>()
                .ReverseMap();

            #endregion

            #region Service

            CreateMap<ServiceDto, ServiceViewModel>()
                .ReverseMap();

            #endregion


            #region Dentist

            CreateMap<DentistListDto, AvailableDentistsResponse>()
                .ReverseMap();

            CreateMap<UserDto, DentistViewModel>()
                .ForMember(x => x.MaximumPrice, opt => opt.MapFrom(x => x.Services.Max(x => x.Price)))
                .ForMember(x => x.MinimumPrice, opt => opt.MapFrom(x => x.Services.Min(x => x.Price)))
                .ReverseMap();

            #endregion

            #region Appointment

            CreateMap<Services.Models.Reservation.AppointmentDto, AppointmentViewModel>()
                .ReverseMap();
            #endregion
        }
    }
}
