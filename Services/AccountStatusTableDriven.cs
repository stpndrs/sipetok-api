namespace sipetok_api.Services
{
    public class AccountStatusTableDriven
    {
        private readonly Dictionary<int, string> codeToStatus = new Dictionary<int, string>
        {
            { 0, "INACTIVE" },
            { 1, "ACTIVE" }
        };

        public string GetStatusName(int status)
        {
            try
            {
                if (!codeToStatus.ContainsKey(status))
                {
                    return "STATUS TIDAK DITEMUKAN";
                }

                return codeToStatus[status];
            }
            catch (Exception)
            {
                return "STATUS TIDAK DITEMUKAN";
            }
        }
    }
}
