namespace sipetok_api.dto.Response
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public AuthResponseDto()
        {

        }

        public AuthResponseDto(string Token)
        {
            this.Token = Token;
        }
    }
}
