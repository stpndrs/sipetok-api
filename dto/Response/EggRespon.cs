using sipetok_api.dto.Response;

namespace sipetok_api.Response
{
    public class EggRespon
    {
        public int Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public double Stock { get; set; }
        public int CategoryId { get; set; }
        public virtual EggCategoryResponseDto? Category { get; set; }

        public EggRespon() { }

        public EggRespon(int Id, DateTime ProductionDate, double Stock, int CategoryId, EggCategoryResponseDto Category)
        {
            this.Id = Id;
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.CategoryId = CategoryId;
            this.Category = Category;
        }
    }
}