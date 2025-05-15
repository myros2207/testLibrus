using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestLibrus.Controllers.Requests;
using TestLibrus.Controllers.Responses;
using TestLibrus.Data;

namespace TestLibrus.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("create")]
    public ActionResult<itemRes_get> CreateItem([FromBody] itemReq_create input)
    {
        var newItem = new Models.Item
        {
            Name = input.Name,
            Description = input.Description,
            UserId = input.UserId
        };
        
        _context.Items.Add(newItem);
        _context.SaveChanges();

        var response = new itemRes_get
        {
            Id = newItem.Id,
            Name = newItem.Name,
            Description = newItem.Description,
            UserId = newItem.UserId
        };

        return CreatedAtAction(nameof(CreateItem), response);
    }

    [HttpGet("get/{id}")]
    public ActionResult<itemRes_get> GetItem(int id)
    {
        var item = _context.Items
            .Include(i => i.User)
            .FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
        itemRes_get itemResponse = new itemRes_get
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            UserId = item.UserId
        };
            
        return itemResponse;
    }
    
    [HttpDelete("delete/{id}")]
    public ActionResult<bool> DeleteItem(int id)
    {
        var item = _context.Items.Find(id);
        if (item == null) return NotFound();
        _context.Items.Remove(item);
        _context.SaveChanges();
        return true;
    }
}