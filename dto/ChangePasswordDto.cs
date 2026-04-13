namespace sipetok_api.dto
{
    public class ChangePasswordDto
    {
        public string password_old { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public ChangePasswordDto() {}
    }
}