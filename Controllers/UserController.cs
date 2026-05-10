using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto;
using sipetok_api.dto.Respon;

[Authorize]
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
    [Authorize(Roles = "1")]
    public IActionResult GetAllUsers()
    {
        try
        {
            var allUser = _mapper.Map<List<UserRespon>>(dbContext.Users.ToList());
            var respon = new ResponData<List<UserRespon>>
            {
                success = true,
                data = allUser,
                message = "Successfully retrieves all user data"
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
    [Authorize(Roles = "1")]
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
                    message = $"User data with id {id} not found"
                });
            }

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = user,
                message = $"Successfully retrieved user data with id {id}"
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
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("myaccount")]
    public IActionResult GetMyAccount()
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var user = _mapper.Map<UserRespon>(dbContext.Users.Find(userId));

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = false,
                    message = $"User data with id {userId} not found"
                });
            }

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = user,
                message = $"Successfully retrieved user data with id {userId}"
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
                message = $"Successfully added user data"
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
                    success = false,
                    message = $"User data with id {id} not found"
                });
            }

            if (!string.IsNullOrWhiteSpace(userDto.password))
            {
                user.password = Bcrypt.BcryptPassword(userDto.password);
            }

            user.username = userDto.username;
            user.email = userDto.email;
            user.status = userDto.status;
            user.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = _mapper.Map<UserRespon>(user),
                message = $"Successfully updated user data with id {id}"
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
    [Route("updatemyaccount")]
    public IActionResult UpdateMyAccount([FromBody] UserDto userDto)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var user = dbContext.Users.Find(userId);

            if (user is null)
            {
                return NotFound(new ResponData<UserRespon>
                {
                    success = false,
                    message = $"User data with id {userId} not found"
                });
            }

            if (!string.IsNullOrWhiteSpace(userDto.password))
            {
                user.password = Bcrypt.BcryptPassword(userDto.password);
            }

            user.username = userDto.username;
            user.email = userDto.email;
            user.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<UserRespon>
            {
                success = true,
                data = _mapper.Map<UserRespon>(user),
                message = $"Successfully updated user data with id {userId}"
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
                    message = $"User data with id {userId} not found"
                });
            }

            if (BCrypt.Net.BCrypt.Verify(changePasswordDto.password_old, user.password))
            {
                return BadRequest(new ResponData<UserRespon>
                {
                    success = false,
                    message = "Old password is incorrect"
                });
            }
            user.password = Bcrypt.BcryptPassword(changePasswordDto.password);
            user.UpdateTimestamps();

            dbContext.SaveChanges();
            return Ok("Password changed successfully");
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