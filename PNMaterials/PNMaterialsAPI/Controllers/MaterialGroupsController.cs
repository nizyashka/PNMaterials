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
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetAll(bool includeArchived = false)
    {
        var query = _db.MaterialGroups.AsQueryable();

        if (!includeArchived)
            query = query.Where(g => !g.IsArchived);

        var groups = await query
            .OrderBy(g => g.Name)
            .Select(g => new MaterialGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsArchived = g.IsArchived,
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
                Description = g.Description,
                IsArchived = g.IsArchived,
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

    //Поместить группу в архив
    [HttpPost("{id:int}/archive")]
    public async Task<ActionResult<MaterialGroupDto>> Archive(int id)
    {
        var group = await _db.MaterialGroups.FindAsync(id);
        if (group is null)
            return NotFound();

        var hasMaterials = await _db.Materials.AnyAsync(m => m.GroupId == id);
        if (hasMaterials)
            return Problem(detail: "Нельзя архивировать группу: она содержит материалы.", statusCode: StatusCodes.Status409Conflict);

        group.IsArchived = true;
        await _db.SaveChangesAsync();
        var groupDto = await ToDtoAsync(group.Id);

        return Ok(groupDto);
    }

    //Восстановить группу из архива
    [HttpPost("{id:int}/restore")]
    public async Task<ActionResult<MaterialGroupDto>> Restore(int id)
    {
        var group = await _db.MaterialGroups.FindAsync(id);
        if (group is null)
            return NotFound();

        group.IsArchived = false;
        await _db.SaveChangesAsync();
        var groupDto = await ToDtoAsync(group.Id);

        return Ok(groupDto);
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
            return Problem(detail: "Нельзя удалить группу: в ней есть материалы.", statusCode: StatusCodes.Status409Conflict);

        _db.MaterialGroups.Remove(group);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Перечитать группу
    private async Task<MaterialGroupDto> ToDtoAsync(int id)
    {
        return await _db.MaterialGroups
            .Where(g => g.Id == id)
            .Select(g => new MaterialGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsArchived = g.IsArchived,
            })
            .FirstAsync();
    }
}