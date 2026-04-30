namespace sipetok_api.dto.Respon
{
    public class AuthRespon
    {
        public string token { get; set; }

        public AuthRespon(string token)
        {
            this.token = token;
        }
    }
}
