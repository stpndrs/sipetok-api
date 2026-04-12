using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class EggCategory
    {
        public int id { get; set; }

        // Gunakan decimal untuk harga
        public decimal price { get; set; }

        [MaxLength(255)]
        public string description { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;

        public EggCategory() { }

        public EggCategory(int id, decimal price, string description, string name)
        {
            this.id = id;
            this.price = price;
            this.description = description;
            this.name = name;
        }
    }
}
