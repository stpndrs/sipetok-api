using sipetok_api.Models;
using System.Collections;

namespace sipetok_api.Services
{
    public class EggStockTableDriven
    {
        public List<string[]> BuildTable(List<Egg> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                Egg item = data[i];
                string[] baris = new string[4];
                baris[0] = item.id.ToString();
                baris[1] = item.production_date.ToString();
                baris[2] = item.stock.ToString();
                baris[3] = item.tenant_id.ToString();
                baris[4] = item.category_id.ToString();

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return ["ID", "tangal produksi", "stock", "tenat_id", "category_id"];
        }
    }
}
//this.id = id;
//this.production_date = production_date;
//this.stock = stock;
//this.tenant_id = tenant_id;
//this.category_id = category_id;
