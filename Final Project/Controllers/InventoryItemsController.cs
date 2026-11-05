using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;

namespace WarehouseManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryItemsController : ControllerBase
{
    private readonly WarehouseDbContext database;

    public InventoryItemsController(WarehouseDbContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> Get(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return await database.InventoryItems.Take(5).ToListAsync();
        }

        // ItemNumber is the public nine-digit lookup value. Fall back to ItemId
        // so existing requests such as ?id=1 remain supported.
        var item = await database.InventoryItems
            .FirstOrDefaultAsync(x => x.ItemNumber == id);

        if (item == null && int.TryParse(id, out var itemId))
        {
            item = await database.InventoryItems.FindAsync(itemId);
        }

        if (item == null) return NotFound();
        return Ok(new List<InventoryItem> { item });
    }

    [HttpGet("item-number/{itemNumber}")]
    public async Task<ActionResult<InventoryItem>> GetByItemNumber(string itemNumber)
    {
        var item = await database.InventoryItems
            .FirstOrDefaultAsync(x => x.ItemNumber == itemNumber);

        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> Post(InventoryItem item)
    {
        database.InventoryItems.Add(item);
        await database.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.ItemId }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, InventoryItem item)
    {
        if (id != item.ItemId) return BadRequest();
        database.Entry(item).State = EntityState.Modified;
        await database.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await database.InventoryItems.FindAsync(id);
        if (item == null) return NotFound();
        database.InventoryItems.Remove(item);
        await database.SaveChangesAsync();
        return NoContent();
    }
}