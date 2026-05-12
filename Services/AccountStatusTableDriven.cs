namespace sipetok_api.Services
{
    public class AccountStatusTableDriven
    {
        public string GetStatusName(int status)
        {
            string[] namaStatus =
            {
                 "INACTIVE",
                  "ACTIVE"
            };

            if (status < 0 || status >= namaStatus.Length)
            {
                return "UNKNOWN STATUS";
            }

            return namaStatus[status];
        }
    }
}