using AutoMapper;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Response;
using sipetok_api.Utilis;
using sipetok_api.Services;

namespace sipetok_api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRequestDto, User>();
            CreateMap<TenantDto, Tenant>();
            CreateMap<OperationalDto, Operational>();
            CreateMap<EggDto, Egg>();
            CreateMap<EggCategoryRequestDto, EggCategory>();
            CreateMap<ChangePasswordDto, User>();
            CreateMap<TransactionDto, Transaction>();
            CreateMap<TransactionDetailDto, TransactionDetail>();

            CreateMap<User, UserResponseDto>();
            CreateMap<Tenant, TenantRespon>();
            CreateMap<Operational, OperationalRespon>();
            CreateMap<Egg, EggRespon>();
            CreateMap<EggCategory, EggCategoryResponseDto>();
            CreateMap<Transaction, TransactionRespon>();
            CreateMap<TransactionDetail, TransactionDetailRespon>();
        }
    }
}