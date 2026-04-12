using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.helper;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        user.password = Bcrypt.BcryptPassword(user.password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsers), new { id = user.id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.id) return BadRequest();

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsers), new { id = user.id }, user);
    }
}