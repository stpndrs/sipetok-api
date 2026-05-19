using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.dto;

namespace sipetok_api.Controllers
{
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
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var allUser = _mapper.Map<List<UserRespon>>(dbContext.Users.ToList());
                var respon = new ResponData<List<UserRespon>>(true, allUser, "Successfully retrieves all user data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var userEntity = dbContext.Users.Find(id);

                if (userEntity is null)
                {
                    return NotFound(new ResponData<object?>(false, $"User data with id {id} not found"));
                }

                var userRespon = _mapper.Map<UserRespon>(userEntity);
                var respon = new ResponData<UserRespon>(true, userRespon, $"Successfully retrieved user data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("myaccount")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetMyAccount()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var userEntity = dbContext.Users.Find(userId);

                if (userEntity is null)
                {
                    return NotFound(new ResponData<object?>(false, $"User data with id {userId} not found"));
                }

                var userRespon = _mapper.Map<UserRespon>(userEntity);
                var respon = new ResponData<UserRespon>(true, userRespon, $"Successfully retrieved user data with id {userId}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddUser([FromBody] UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                if (string.IsNullOrWhiteSpace(user.password))
                {
                    return BadRequest(new ResponData<object?>(false, "Password is required"));
                }
                user.password = Bcrypt.BcryptPassword(user.password);

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                var respon = new ResponData<UserRespon>(true, _mapper.Map<UserRespon>(user), "Successfully added user data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                var user = dbContext.Users.Find(id);

                if (user is null)
                {
                    return NotFound(new ResponData<object?>(false, $"User data with id {id} not found"));
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

                var respon = new ResponData<UserRespon>(true, _mapper.Map<UserRespon>(user), $"Successfully updated user data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

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
                    return NotFound(new ResponData<object?>(false, $"User data with id {userId} not found"));
                }

                if (!string.IsNullOrWhiteSpace(userDto.password))
                {
                    user.password = Bcrypt.BcryptPassword(userDto.password);
                }

                user.username = userDto.username;
                user.email = userDto.email;
                user.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<UserRespon>(true, _mapper.Map<UserRespon>(user), $"Successfully updated user data with id {userId}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

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
                    return NotFound(new ResponData<object?>(false, $"User data with id {userId} not found"));
                }

                if (BCrypt.Net.BCrypt.Verify(changePasswordDto.password_old, user.password))
                {
                    return BadRequest(new ResponData<object?>(false, "Old password is incorrect"));
                }
                user.password = Bcrypt.BcryptPassword(changePasswordDto.password);
                user.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Password changed successfully");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = dbContext.Users.Find(id);

                if (user is null)
                {
                    return NotFound(new ResponData<object?>(false, $"User data with id {id} not found"));
                }

                user.SoftDelete();
                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Successfully deleted user data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }
    }
}