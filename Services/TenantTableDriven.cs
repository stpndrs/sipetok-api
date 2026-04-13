using sipetok_api.Models;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class TenanTableDriven
    {
        public List<string[]> BuildTable(List<Tenant> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                Tenant tenant = data[i];
                string[] baris = new string[4];
                baris[0] = tenant.id.ToString();
                baris[1] = tenant.name;
                baris[2] = tenant.address;
                baris[3] = tenant.phoneNumber;

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return  [ "ID", "Nama Tenan", "Alamat", "No. Telepon" ];
        }
    }
}