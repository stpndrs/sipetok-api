using AutoMapper;
using sipetok_api.dto;
using sipetok_api.Models;

namespace sipetok_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<TenantDto, Tenant>();
            CreateMap<OperationalDto, Operational>();
            CreateMap<EggDto, Egg>();
            CreateMap<EggCategoryDto, EggCategory>();
            CreateMap<ChangePasswordDto, User>();
        }
    }
}