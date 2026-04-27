using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace sipetok_api.Utilis
{
    public enum PaymentState
    {
        Pending,    // Transaksi dibuat & menunggu aksi
        Processing, // Sedang divalidasi atau diproses sistem
        Success,    // Lunas & berhasil disimpan
        Failed,     // Gagal (validasi, sistem, atau simpan)
        Cancelled   // Dibatalkan oleh pengguna
    }
}
