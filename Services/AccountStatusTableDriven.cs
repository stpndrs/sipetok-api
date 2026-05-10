namespace sipetok_api.Services
{
    public class AccountStatusTableDriven
    {
        int[] statusAwal =
        {
            1,
            0
        };

        string[] aksi =
        {
            "NONAKTIFKAN",
            "AKTIFKAN"
        };

        int[] statusAkhir =
        {
            0,
            1
        };

        public int GetNextStatus(int currentStatus, string action)
        {
            for (int i = 0; i < statusAwal.Length; i++)
            {
                if (statusAwal[i] == currentStatus && aksi[i] == action.ToUpper())
                {
                    return statusAkhir[i];
                }
            }

            return currentStatus;
        }

        public string GetStatusName(int status)
        {
            string[] namaStatus =
            {
                "Tidak Aktif",
                "Aktif"
            };

            return namaStatus[status];
        }
    }
}
