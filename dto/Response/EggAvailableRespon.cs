<<<<<<<< HEAD:dto/Response/EggAvailableRespon.cs
using sipetok_api.dto.Respon;
========
>>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7:dto/Response/EggAvailableResponseDto.cs
using sipetok_api.dto.Response;

namespace sipetok_api.dto.Response
{
    public class EggAvailableResponseDto
    {
        public double Stock { get; set; }
        public int TenantId { get; set; }
        public virtual TenantResponseDto? Tenant { get; set; }
        public int CategoryId { get; set; }
        public virtual EggCategoryResponseDto? Category { get; set; }

        public string CategoryName { get; set; }
        public string TenantName { get; set; }

        public EggAvailableResponseDto() { }

        public EggAvailableResponseDto(double Stock, int TenantId, int CategoryId, string CategoryName, string TenantName)
        {
            this.Stock = Stock;
            this.TenantId = TenantId;
            this.CategoryId = CategoryId;
            this.CategoryName = CategoryName;
            this.TenantName = TenantName;
        }
    }
}