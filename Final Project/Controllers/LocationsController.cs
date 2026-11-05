using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;

namespace WarehouseManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly WarehouseDbContext database;

    public LocationsController(WarehouseDbContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> Get(int? id)
    {
        if (id == null || id == 0)
            return await database.Locations.Take(5).ToListAsync();

        var location = await database.Locations.FindAsync(id.Value);
        if (location == null) return NotFound();
        return Ok(new List<Location> { location });
    }

    [HttpPost]
    public async Task<ActionResult<Location>> Post(Location location)
    {
        database.Locations.Add(location);
        await database.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = location.LocationId }, location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Location location)
    {
        if (id != location.LocationId) return BadRequest();
        database.Entry(location).State = EntityState.Modified;
        await database.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await database.Locations.FindAsync(id);
        if (location == null) return NotFound();
        database.Locations.Remove(location);
        await database.SaveChangesAsync();
        return NoContent();
    }
}