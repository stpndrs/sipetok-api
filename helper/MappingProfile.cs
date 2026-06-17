using AutoMapper;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using sipetok_api.Respon;
using sipetok_api.Utilis;
using sipetok_api.Services;

namespace sipetok_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequestDto, User>();
            CreateMap<UserRequestDto, User>();
            CreateMap<User, UserResponseDto>();
            CreateMap<TenantDto, Tenant>();
            CreateMap<OperationalDto, Operational>();
            CreateMap<EggInventoryRequestDto, EggInventory>();
            CreateMap<EggCategoryRequestDto, EggCategory>();
            CreateMap<ChangePasswordDto, User>();
            CreateMap<TransactionRequestDto, Transaction>();
            CreateMap<TransactionDetail, TransactionDetailResponseDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<TransactionDetailDto, TransactionDetail>();

            CreateMap<User, UserResponseDto>();
            CreateMap<Tenant, TenantRespon>();
            CreateMap<Operational, OperationalResponseDto>();
            CreateMap<EggInventory, EggInventoryResponseDto>();
            CreateMap<EggCategory, EggCategoryResponseDto>();
            CreateMap<Transaction, TransactionResponseDto>();
            CreateMap<TransactionDetail, TransactionDetailResponseDto>();
        }
    }
}