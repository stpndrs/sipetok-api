namespace sipetok_api.dto.Response
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
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
