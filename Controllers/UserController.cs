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

        user.username = userDto.username;
        user.password = userDto.password;
        user.email = userDto.email;
        user.role = userDto.role;
        user.status = userDto.status;

        dbContext.SaveChanges();
        return Ok(user);
    }
}