namespace sipetok_api.dto.Respon
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
