using sipetok_api.Models;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class AkunTableDriven
    {
        public List<string[]> BuildTable(List<User> data)
        {
            List<string[]> tabel = new List<string[]>();

            for (int i = 0; i < data.Count; i++)
            {
                User user = data[i];
                string[] baris = new string[5];
                baris[0] = user.id.ToString();
                baris[1] = user.username;
                baris[2] = user.email;
                baris[3] = user.role.ToString();
                baris[4] = user.status.ToString();

                tabel.Add(baris);
            }

            return tabel;
        }

        public string[] GetHeader()
        {
            return [ "ID", "Username", "Email", "Role", "Status" ];
        }
    }
}