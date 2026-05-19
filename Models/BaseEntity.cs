namespace sipetok_api.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void UpdateTimestamps()
        {
            UpdatedAt = DateTime.Now;
        }
        public void SoftDelete()
        {
            DeletedAt = DateTime.Now;
        }
    }
}