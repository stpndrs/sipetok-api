namespace sipetok_api.Services
{
    public class AccountRoleTableDriven
    {
        private readonly Dictionary<int, string> codeToRole = new Dictionary<int, string>
        {
            { 1, "ADMIN" },
            { 2, "TENANT" },
            { 3, "CUSTOMER" }
        };

        public string GetRoleName(int kode)
        {
            try
            {
                if (!codeToRole.ContainsKey(kode))
                {
                    return "ROLE TIDAK DITEMUKAN";
                }

                return codeToRole[kode];
            }
            catch (Exception)
            {
                return "ROLE TIDAK DITEMUKAN";
            }
        }

        public int GetRoleCode(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    return 0;
                }

                string normalizedRole = role.ToUpper();

                var data = codeToRole.FirstOrDefault(x => x.Value == normalizedRole);

                if (data.Equals(default(KeyValuePair<int, string>)))
                {
                    return 0;
                }

                return data.Key;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
