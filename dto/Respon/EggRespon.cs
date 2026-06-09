using sipetok_api.dto.Respon;

namespace sipetok_api.Respon
{
    public class EggRespon
    {
        public int Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public double Stock { get; set; }
        public int CategoryId { get; set; }
        public virtual EggCategoryRespon? Category { get; set; }

        public EggRespon() { }

        public EggRespon(int Id, DateTime ProductionDate, double Stock, int CategoryId, EggCategoryRespon Category)
        {
            this.Id = Id;
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.CategoryId = CategoryId;
            this.Category = Category;
        }
    }
}