using sipetok_api.Models;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class PenjualanTableDriven
    {
        public List<string[]> BuildTable(List<Transaction> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                Transaction transaction = data[i];
                string[] baris = new string[5];
                baris[0] = transaction.id.ToString();
                baris[1] = transaction.date.ToString();
                baris[2] = transaction.customer.name;
                baris[3] = transaction.total_price.ToString();
                baris[4] = transaction.payment_amount.ToString();

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return [ "ID Transaksi", "Tanggal", "Pelanggan", "Total", "Dibayar" ];
        }
    }
}