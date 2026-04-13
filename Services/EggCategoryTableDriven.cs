using sipetok_api.Models;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class KategoriTelurTableDriven
    {
        public List<string[]> BuildTable(List<EggCategory> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                EggCategory item = data[i];
                string[] baris = new string[4];
                baris[0] = item.id.ToString();
                baris[1] = item.name;
                baris[2] = item.price.ToString();
                baris[3] = item.description;

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return  [ "ID", "Nama Kategori", "Harga", "Deskripsi" ];
        }
    }
}