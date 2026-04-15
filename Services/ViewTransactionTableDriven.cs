using sipetok_api.Models;

namespace sipetok_api.Services
{
    public class ViewTransactionTableDriven
    {
        public List<string[]> BuildTable(List<TransactionDetail> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                TransactionDetail item = data[i];
                string[] baris = new string[4];
                baris[0] = item.id.ToString();
                baris[1] = item.transaction_id.ToString();
                baris[2] = item.category_name;
                baris[3] = item.quantity.ToString();
                baris[4] = item.subtotal.ToString();

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return ["ID", "ID transaksi", "nama kategori", "quantity", "subtotal"];
        }
    }
}

