using AutoMapper;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Respon;

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
            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionDetail, TransactionDetailDto>();

            CreateMap<User, UserRespon>();
            CreateMap<Customer, CustomerRespon>();
            CreateMap<Tenant, TenantRespon>();
            CreateMap<Operational, OperationalRespon>();
            CreateMap<Egg, EggRespon>();
            CreateMap<EggCategory, EggCategoryRespon>();
            CreateMap<Transaction, TransactionRespon>();
            CreateMap<TransactionDetail, TransactionDetailRespon>();
        }
    }
}