using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.dto;
using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.helper;
using AutoMapper;

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
        var allUser = dbContext.Users.ToList();
        return Ok(allUser);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetUserById(int id)
    {
        var user = dbContext.Users.Find(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult AddUser(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        user.password = Bcrypt.BcryptPassword(user.password);

        dbContext.Users.Add(user);
        dbContext.SaveChanges();

        return Ok(user);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateUser(int id, UserDto userDto)
    {
        var user = dbContext.Users.Find(id);

        if (user is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(userDto.username))
            user.username = userDto.username;

        if (!string.IsNullOrEmpty(userDto.password))
            user.password = userDto.password;

        if (!string.IsNullOrEmpty(userDto.email))
            user.email = userDto.email;

        if (userDto.status != null)
            user.status = userDto.status;

        dbContext.SaveChanges();
        return Ok(user);
    }
}