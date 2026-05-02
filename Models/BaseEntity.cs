namespace sipetok_api.Models
{
    public abstract class BaseEntity
    {
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }

        public void UpdateTimestamps()
        {
            updated_at = DateTime.Now;
        }
        public void SoftDelete()
        {
            deleted_at = DateTime.Now;
        }
    }
}