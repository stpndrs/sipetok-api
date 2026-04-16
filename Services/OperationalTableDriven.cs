using sipetok_api.Models;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class OperasionalTableDriven
    {
        public List<string[]> BuildTable(List<Operational> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                Operational item = data[i];
                string[] baris = new string[4];
                baris[0] = item.id.ToString();
                baris[1] = item.operational_date.ToString();
                baris[2] = item.name;
                baris[3] = item.operational_cost;

                tabel.Add(baris);
            }

            return tabel;
        }
        public string[] GetHeader()
        {
            return ["ID", "Tanggal", "Jenis Operasional", "Biaya"];
        }
    }
}