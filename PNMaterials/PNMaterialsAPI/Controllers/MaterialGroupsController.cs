using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;
using System.Security.Claims;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialGroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MaterialGroupsController(AppDbContext db)
    {
        _db = db;
    }

    private IQueryable<MaterialGroupDto> ToDto(IQueryable<MaterialGroup> groups)
    {
        return groups
            .Select(g => new MaterialGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Audit = new AuditInfoDto
                {
                    CreatedByDisplayName = g.Audit.CreatedByUserId == null ? "Система" : _db.Users
                    .Where(u => u.Id == g.Audit.CreatedByUserId)
                    .Select(u => u.LastName + " " + u.FirstName)
                    .FirstOrDefault() ?? "Пользователь не найден",

                    CreatedAt = g.Audit.CreatedAt,

                    EditedByDisplayName = _db.Users
                    .Where(u => u.Id == g.Audit.EditedByUserId)
                    .Select(u => u.LastName + " " + u.FirstName)
                    .FirstOrDefault(),

                    EditedAt = g.Audit.EditedAt
                }
            });
    }

    // Список всех групп
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetAll()
    {
        var groups = await ToDto(_db.MaterialGroups.OrderBy(g => g.Name)).ToListAsync();

        return Ok(groups);
    }

    // Одна группа по Id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialGroupDto>> GetById(int id)
    {
        var group = await ToDto(_db.MaterialGroups.Where(g => g.Id == id)).SingleOrDefaultAsync();

        if (group is null)
            return NotFound();

        return Ok(group);
    }

    // Создать
    [HttpPost]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<ActionResult<MaterialGroupDto>> Create(MaterialGroupEditDto dto)
    {
        var name = dto.Name.Trim();
        if (await _db.MaterialGroups.AnyAsync(g => g.Name == name))
            return Problem(detail: "Группа материалов с таким именем уже существует.", statusCode: StatusCodes.Status409Conflict);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося создать группу материалов.", statusCode: StatusCodes.Status500InternalServerError);

        var group = new MaterialGroup
        {
            Name = name,
            Description = dto.Description,
            Audit = new AuditInfo
            {
                CreatedByUserId = id,
                CreatedAt = DateTime.Now
            }
        };

        _db.MaterialGroups.Add(group);
        await _db.SaveChangesAsync();

        var result = await ToDto(_db.MaterialGroups.Where(g => g.Id == group.Id)).SingleAsync();

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, result);
    }

    // Изменить
    [HttpPut("{id:int}")]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<IActionResult> Update(int id, MaterialGroupEditDto dto)
    {
        var group = await _db.MaterialGroups.FindAsync(id);
        if (group is null)
            return NotFound();

        var name = dto.Name.Trim();
        if ((name, dto.Description) == (group.Name, group.Description))
            return NoContent();

        if (await _db.MaterialGroups.AnyAsync(g => g.Name == name && g.Id != id))
            return Problem(detail: "Группа материалов с таким именем уже существует.", statusCode: StatusCodes.Status409Conflict);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося создать изменить материалов.", statusCode: StatusCodes.Status500InternalServerError);

        group.Name = name;
        group.Description = dto.Description;
        group.Audit = group.Audit with { EditedByUserId = userId, EditedAt = DateTime.Now };

        await _db.SaveChangesAsync();

        return NoContent();
    }
}