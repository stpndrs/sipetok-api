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
            var allUser = _mapper.Map<List<UserRespon>>(dbContext.Users.ToList());
            var respon = new ResponData<List<UserRespon>>
            {
                success = true,
                data = allUser,
                message = "Beehasil mengambil semua data user"
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                success = true,
                message = ex.Message
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
                    success = false,
                    message = $"Data user dengan id {id} tidak ditemukan"
                });
            }

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = user,
                message = $"Berhasil mengambil data user pada id {id}"
            };

            return Ok(Response);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                success = false,
                message = ex.Message
            };
            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddUser([FromBody] UserDto userDto)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            if (string.IsNullOrWhiteSpace(user.password))
            {
                return BadRequest(new ResponData<UserRespon>
                {
                    success = false,
                    message = "Password is required"
                });
            }
            user.password = Bcrypt.BcryptPassword(user.password);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = _mapper.Map<UserRespon>(user),
                message = $"Berhasil menambahkan data customer"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                success = false,
                message = ex.Message
            };
            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateUser(int id, [FromBody] UserDto userDto)
    {
        try
        {
            var user = dbContext.Users.Find(id);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = true,
                    message = $"Data user dengan id {id} tidak ditemukan"
                });
            }

            if (string.IsNullOrWhiteSpace(userDto.password))
            {
                return BadRequest(new ResponData<UserRespon>
                {
                    success = false,
                    message = "Password is required"
                });
            }
            user.username = userDto.username;
            user.password = Bcrypt.BcryptPassword(userDto.password);
            user.email = userDto.email;
            user.status = userDto.status;
            user.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = _mapper.Map<UserRespon>(user),
                message = "Berhasil memperbarui data"
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                success = true,
                message = ex.Message
            };
            return Ok(respon);
        }
    }

    [HttpPut]
    [Route("changepassword/{userId:int}")]
    public IActionResult ChangePassword(int userId, [FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = dbContext.Users.Find(userId);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = false,
                    message = $"Data user dengan id {userId} tidak ditemukan"
                });
            }

            if (BCrypt.Net.BCrypt.Verify(changePasswordDto.password_old, user.password))
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = false,
                    message = "Password lama salah"
                });
            }
            user.password = Bcrypt.BcryptPassword(changePasswordDto.password);
            user.UpdateTimestamps();

            dbContext.SaveChanges();
            return Ok("Password berhasil diubah");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            var user = dbContext.Users.Find(id);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = false,
                    message = $"User data with id {id} not found"
                });
            }

            user.SoftDelete();
            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                success = true,
                message = "Successfully deleted user data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<UserRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}