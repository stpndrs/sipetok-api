using sipetok_api.Models;
using sipetok_api.Utilis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class Customer
    {
        public int id { get; set; }

        [Required, MaxLength(100)]
        public string name { get; set; } = string.Empty;

        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual User? user { get; set; }

        [MaxLength(255)]
        public string address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string phone_number { get; set; } = string.Empty;

        public Customer() { }
        public sipetok_api.Utilis.CustomerStatus status { get; set; }
        public Customer(int id, string name, int user_id, string address, string phone_number)
        {
            this.id = id;
            this.name = name;
            this.user_id = user_id;
            this.address = address;
            this.phone_number = phone_number;
        }
    }
}
