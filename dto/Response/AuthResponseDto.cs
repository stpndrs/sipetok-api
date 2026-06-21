namespace sipetok_api.dto.Response
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public int Role { get; set; }

        public AuthResponseDto()
        {

        }

        public AuthResponseDto(string Token, string Username, int Role)
        {
            this.Token = Token;
            this.Username = Username;
            this.Role = Role;
        }

        public AuthResponseDto(string Token)
        {
            this.Token = Token;
        }
    }
}
