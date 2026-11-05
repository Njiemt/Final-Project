using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;

namespace WarehouseManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamMembersController : ControllerBase
{
    private readonly WarehouseDbContext database;

    public TeamMembersController(WarehouseDbContext database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamMember>>> Get(int? id)
    {
        if (id == null || id == 0)
        {
            return await database.TeamMembers.Take(5).ToListAsync();
        }

        var member = await database.TeamMembers.FindAsync(id.Value);
        if (member == null) return NotFound();
        return Ok(new List<TeamMember> { member });
    }

    [HttpPost]
    public async Task<ActionResult<TeamMember>> Post(TeamMember member)
    {
        database.TeamMembers.Add(member);
        await database.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = member.MemberId }, member);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, TeamMember member)
    {
        if (id != member.MemberId) return BadRequest();
        database.Entry(member).State = EntityState.Modified;
        await database.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await database.TeamMembers.FindAsync(id);
        if (member == null) return NotFound();
        database.TeamMembers.Remove(member);
        await database.SaveChangesAsync();
        return NoContent();
    }
}