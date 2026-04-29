using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.helper;
using AutoMapper;
using sipetok_api.dto;
using sipetok_api.dto.Respon;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public UserController(AppDbContext context, IMapper mapper)
    {
        dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        try
        {
            var allUser = _mapper.Map<UserRespon>(dbContext.Users.ToList());
            var respon = new ResponData<UserRespon>
            {
                status = true,
                data = allUser,
                message = new List<string> { "Beehasil mengambil semua data user" }
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                status = true,
                message = new List<string> { ex.Message }
            };
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = _mapper.Map<UserRespon>(dbContext.Users.Find(id));

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    status = false,
                    message = new List<string> { $"Data user dengan id {id} tidak ditemukan" }
                });
            }

            var respon = new ResponData<UserRespon>
            {
                status = true,
                data = user,
                message = new List<string> { $"Berhasil mengambil data user pada id {id}" }
            };

            return Ok(Response);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };
            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddUser(UserDto userDto)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            user.password = Bcrypt.BcryptPassword(user.password);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                status = true,
                data = _mapper.Map<UserRespon>(user),
                message = new List<string> { $"Berhasil menambahkan data customer" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };
            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateUser(int id, UserDto userDto)
    {
        try
        {
            var user = dbContext.Users.Find(id);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    status = true,
                    message = new List<string> { $"Data user dengan id {id} tidak ditemukan" }
                });
            }

            if (!string.IsNullOrEmpty(userDto.username))
                user.username = userDto.username;

            if (!string.IsNullOrEmpty(userDto.password))
                user.password = userDto.password;

            if (!string.IsNullOrEmpty(userDto.email))
                user.email = userDto.email;

            if (userDto.status != user.status)
                user.status = userDto.status;

            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                status = true,
                data = _mapper.Map<UserRespon>(user),
                message = new List<string> { "Berhasil memperbarui data" }
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                status = true,
                message = new List<string> { ex.Message }
            };
            return Ok(respon);
        }
    }

    [HttpPut("changepassword/{userId:int}")]
    public IActionResult ChangePassword(int userId, ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ResponData<UserRespon>
            {
                status = false,
                message = errors
            });
        }

        try
        {
            var user = dbContext.Users.Find(userId);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    status = false,
                    message = new List<string> { $"Data user dengan id {userId} tidak ditemukan" }
                });
            }

            if (BCrypt.Net.BCrypt.Verify(changePasswordDto.password_old, user.password))
            {
                return NotFound(new ResponData<UserRespon>
                {
                    status = false,
                    message = new List<string> { "Password lama salah" }
                });
            }
            user.password = Bcrypt.BcryptPassword(changePasswordDto.password);

            dbContext.SaveChanges();
            return Ok("Password berhasil diubah");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}