using AutoMapper;
using Dental_Clinic.Requests.User;
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

            #endregion
        }
    }
}
