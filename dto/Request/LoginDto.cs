using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.dto.Request
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
