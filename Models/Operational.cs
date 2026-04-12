using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class Operational
    {
        public int id { get; set; }

        [Required, MaxLength(100)]
        public string name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string operational_cost { get; set; } = string.Empty; // Atau ubah ke decimal jika ini angka

        public int tenant_id { get; set; }

        [ForeignKey("tenant_id")]
        public virtual Tenant? tenant { get; set; }
        public DateTime operational_date { get; set; }

        public Operational() { }

        public Operational(int id, string name, string operational_cost, int tenant_id, DateTime operational_date)
        {
            this.id = id;
            this.name = name;
            this.operational_cost = operational_cost;
            this.tenant_id = tenant_id;
            this.operational_date = operational_date;
        }
    }
}
