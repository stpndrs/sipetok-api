using BCrypt.Net;

namespace sipetok_api.helper
{
    
}
public static class Bcrypt
{
    public static string BcryptPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}