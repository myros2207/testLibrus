using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestLibrus.Controllers.Requests;
using TestLibrus.Controllers.Responses;
using TestLibrus.Data;

namespace TestLibrus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("create")]
    public ActionResult<UserRes_create> CreateUser([FromBody] UserReq_create input)
    {
        var newUser = new Models.User
        {
            Name = input.name
        };
        _context.Users.Add(newUser);
        _context.SaveChanges();

        var response = new UserRes_create
        {
            Id = newUser.Id,
            Name = newUser.Name
        };

        return CreatedAtAction(nameof(CreateUser), response);
    }

    [HttpGet("get/{id}")]
    public ActionResult<UserRes_get> GetUser(int id)
    {
        var User = _context.Users
            .Include(p => p.Items)
            .FirstOrDefault(p => p.Id == id);
        if (User == null) return NotFound();
        UserRes_get UserResponse = new UserRes_get
        {
            Id = User.Id,
            Name = User.Name,
            BirthDate = User.BirthDate,
            DeathDate = User.DeathDate,
            Items = User.Items.Select(i => new itemRes_get
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
            }).ToList()
        };

        return UserResponse;
    }

    [HttpPut("update/{id}")]
    public ActionResult<UserRes_create> UpdateUser(int id, [FromBody] UserReq_create input)
    {
        var User = _context.Users.Find(id);
        if (User == null)
        {
            return NotFound();
        }
        User.Name = input.name;
        _context.SaveChanges();
        return Ok(User);
    }

    [HttpDelete("delete/{id}")]
    public ActionResult<bool> DeleteUser(int id)
    {
        var User = _context.Users
            .Include(p => p.Items)
            .FirstOrDefault(p => p.Id == id);
        if (User == null)
        {
            return NotFound();
        }

        if (User.Items != null && User.Items.Any())
        {
            return false;
        }
        _context.Users.Remove(User);
        _context.SaveChanges();
        return true;
    }
}