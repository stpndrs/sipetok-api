namespace sipetok_api.Services
{
    public class AccountRoleTableDriven
    {
        string[] namaRole =
        {
            "ADMIN",
            "TENANT",
            "CUSTOMER"
        };

        int[] kodeRole =
        {
            1,
            2,
            3
        };

        public int GetKodeRole(string role)
        {
            for (int i = 0; i < namaRole.Length; i++)
            {
                if (namaRole[i] == role.ToUpper())
                {
                    return kodeRole[i];
                }
            }

            return 0;
        }

        public string GetNamaRole(int kode)
        {
            for (int i = 0; i < kodeRole.Length; i++)
            {
                if (kodeRole[i] == kode)
                {
                    return namaRole[i];
                }
            }

            return "ROLE TIDAK DITEMUKAN";
        }
    }
}
