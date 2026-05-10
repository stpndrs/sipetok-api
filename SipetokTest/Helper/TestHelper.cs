using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api;
using sipetok_api.Data;
using System.Security.Claims;

namespace SipetokTest.Helper
{
    public static class TestHelper
    {
        // Dipakai untuk membuat database sementara di memory saat unit test berjalan
        public static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        // Dipakai untuk membuat AutoMapper agar mapping di controller tetap bisa digunakan saat test
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            return config.CreateMapper();
        }

        // Dipakai untuk method controller yang membaca User.FindFirst("userId")
        public static void SetUserId(ControllerBase controller, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }
    }
}
