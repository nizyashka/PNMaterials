using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialGroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MaterialGroupsController(AppDbContext db)
    {
        _db = db;
    }

    // Список всех групп
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetAll()
    {
        var groups = await _db.MaterialGroups
            .OrderBy(g => g.Name)
            .Select(g => new MaterialGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description
            })
            .ToListAsync();

        return Ok(groups);
    }

    // Одна группа по Id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialGroupDto>> GetById(int id)
    {
        var group = await _db.MaterialGroups
            .Where(g => g.Id == id)
            .Select(g => new MaterialGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description
            })
            .FirstOrDefaultAsync();

        if (group is null)
            return NotFound();

        return Ok(group);
    }

    // Создать
    [HttpPost]
    public async Task<ActionResult<MaterialGroupDto>> Create(MaterialGroupEditDto dto)
    {
        var group = new MaterialGroup
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _db.MaterialGroups.Add(group);
        await _db.SaveChangesAsync();

        var result = new MaterialGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description
        };

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, result);
    }

    // Изменить
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MaterialGroupEditDto dto)
    {
        var group = await _db.MaterialGroups.FindAsync(id);
        if (group is null)
            return NotFound();

        group.Name = dto.Name;
        group.Description = dto.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Удалить
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _db.MaterialGroups.FindAsync(id);
        if (group is null)
            return NotFound();

        var hasMaterials = await _db.Materials.AnyAsync(m => m.GroupId == id);
        if (hasMaterials)
            return Conflict("Нельзя удалить группу: в ней есть материалы.");

        _db.MaterialGroups.Remove(group);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}