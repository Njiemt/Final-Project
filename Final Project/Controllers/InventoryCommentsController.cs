using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;

namespace WarehouseManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryCommentsController : ControllerBase
{
    private readonly WarehouseDbContext database;

    public InventoryCommentsController(WarehouseDbContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryComment>>> Get(int? id)
    {
        if (id == null || id == 0)
        {
            return await database.InventoryComments.Take(5).ToListAsync();
        }

        var comment = await database.InventoryComments.FindAsync(id.Value);
        if (comment == null) return NotFound();
        return Ok(new List<InventoryComment> { comment });
    }

    [HttpPost]
    public async Task<ActionResult<InventoryComment>> Post(InventoryComment comment)
    {
        database.InventoryComments.Add(comment);
        await database.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = comment.CommentId }, comment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, InventoryComment comment)
    {
        if (id != comment.CommentId) return BadRequest();
        database.Entry(comment).State = EntityState.Modified;
        await database.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await database.InventoryComments.FindAsync(id);
        if (comment == null) return NotFound();
        database.InventoryComments.Remove(comment);
        await database.SaveChangesAsync();
        return NoContent();
    }
}