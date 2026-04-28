using sipetok_api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class Tenant
    {
        public int id { get; set; }

        [Required, MaxLength(100)]
        public string name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string phoneNumber { get; set; } = string.Empty;

        [ForeignKey("user_id")]
        public int user_id { get; set; }


        public Tenant() { } // Constructor Kosong

        public Tenant(int id, string name, string address, string phoneNumber, int user_id)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
            this.user_id = user_id;
        }
    }
}
