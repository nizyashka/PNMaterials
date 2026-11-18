using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Services;
using System.Security.Claims;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMaterialCodeGenerator _codeGenerator;

    public MaterialsController(AppDbContext db, IMaterialCodeGenerator codeGenerator)
    {
        _db = db;
        _codeGenerator = codeGenerator;
    }

    private IQueryable<MaterialDto> ToDto(IQueryable<Material> materials)
    {
        return materials
            .Select(m => new MaterialDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Description = m.Description,
                IsArchived = m.IsArchived,
                UnitId = m.UnitId,
                UnitName = m.Unit.Name,
                UnitAllowsFractions = m.Unit.AllowsFractions,
                GroupId = m.GroupId,
                GroupName = m.Group.Name,
                Audit = new AuditInfoDto
                {
                    CreatedByDisplayName = m.Audit.CreatedByUserId == null ? "Система" : _db.Users
                    .Where(u => u.Id == m.Audit.CreatedByUserId)
                    .Select(u => u.LastName + " " + u.FirstName)
                    .FirstOrDefault() ?? "Пользователь не найден",

                    CreatedAt = m.Audit.CreatedAt,

                    EditedByDisplayName = _db.Users
                    .Where(u => u.Id == m.Audit.EditedByUserId)
                    .Select(u => u.LastName + " " + u.FirstName)
                    .FirstOrDefault(),

                    EditedAt = m.Audit.EditedAt
                }
            });
    }

    // Проверка, что переданные единица и группа реально существуют в БД
    private async Task<string?> ValidateReferencesAsync(MaterialEditDto dto)
    {
        if (!await _db.Units.AnyAsync(u => u.Id == dto.UnitId))
            return "Указанная единица измерения не найдена.";

        if (!await _db.MaterialGroups.AnyAsync(g => g.Id == dto.GroupId))
            return "Указанная группа материалов не найдена.";

        return null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll(int? groupId, bool includeArchived = false)
    {
        var query = _db.Materials.AsQueryable();

        if (groupId.HasValue)
            query = query.Where(m => m.GroupId == groupId.Value);

        if (!includeArchived)
            query = query.Where(m => !m.IsArchived);

        var materials = await ToDto(query.OrderBy(m => m.Code)).ToListAsync();

        return Ok(materials);
    }

    // Один материал по id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialDto>> GetById(int id)
    {
        MaterialDto? material = await ToDto(_db.Materials.Where(m => m.Id == id)).FirstOrDefaultAsync();

        if (material is null)
            return NotFound();

        return Ok(material);
    }

    // Создать
    [HttpPost]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<ActionResult<MaterialDto>> Create(MaterialEditDto dto)
    {
        var error = await ValidateReferencesAsync(dto);
        if (error is not null)
            return Problem(detail: error, statusCode: StatusCodes.Status400BadRequest);

        var name = dto.Name.Trim();
        if (await _db.Materials.AnyAsync(m => m.Name == name))
            return Problem(detail: "Материал с таким именем уже существует.", statusCode: StatusCodes.Status409Conflict);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося создать материал.", statusCode: StatusCodes.Status500InternalServerError);

        var material = new Material
        {
            Code = await _codeGenerator.NextAsync(),
            Name = name,
            Description = dto.Description,
            UnitId = dto.UnitId,
            GroupId = dto.GroupId,
            Audit = new AuditInfo { CreatedByUserId = id, CreatedAt = DateTime.Now }
        };

        _db.Materials.Add(material);
        await _db.SaveChangesAsync();

        var result = await ToDto(_db.Materials.Where(m => m.Id == material.Id)).SingleAsync();
        return CreatedAtAction(nameof(GetById), new { id = material.Id }, result);
    }

    // Изменить
    [HttpPut("{id:int}")]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<IActionResult> Update(int id, MaterialEditDto dto)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material is null)
            return NotFound();

        if (material.IsArchived)
            return Problem("Материал архивирован. Для редактирования восстановите его из архива", statusCode: StatusCodes.Status409Conflict);

        if ((dto.Name.Trim(), dto.Description, dto.UnitId, dto.GroupId) == (material.Name, material.Description, material.UnitId, material.GroupId))
            return NoContent();

        var error = await ValidateReferencesAsync(dto);
        if (error is not null)
            return Problem(detail: error, statusCode: StatusCodes.Status400BadRequest);

        var name = dto.Name.Trim();
        if (await _db.Materials.AnyAsync(m => m.Name == name && m.Id != id))
            return Problem(detail: "Материал с таким именем уже существует.", statusCode: StatusCodes.Status409Conflict);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося изменить материал.", statusCode: StatusCodes.Status500InternalServerError);

        material.Name = name;
        material.Description = dto.Description;
        material.UnitId = dto.UnitId;
        material.GroupId = dto.GroupId;
        material.Audit = material.Audit with { EditedByUserId = userId, EditedAt = DateTime.Now };

        await _db.SaveChangesAsync();
        return NoContent();
    }

    //Поместить материал в архив
    [HttpPost("{id:int}/archive")]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<ActionResult<MaterialDto>> Archive(int id)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material is null)
            return NotFound();

        if (material.IsArchived)
            return await GetById(id);

        var usedInRequests = await _db.PurchaseRequestItems.AnyAsync(i => i.MaterialId == id);
        if (usedInRequests)
            return Problem(detail: "Нельзя архивировать материал: он используется в заявках.", statusCode: StatusCodes.Status409Conflict);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося архивировать материал.", statusCode: StatusCodes.Status500InternalServerError);

        material.IsArchived = true;
        material.Audit = material.Audit with { EditedByUserId = userId, EditedAt = DateTime.Now };

        await _db.SaveChangesAsync();
        var materialDto = await ToDto(_db.Materials.Where(m => m.Id == material.Id)).SingleAsync();

        return Ok(materialDto);
    }

    //Восстановить материал из архива
    [HttpPost("{id:int}/restore")]
    [Authorize(Roles = PNRoles.ReadAndWrite)]
    public async Task<ActionResult<MaterialDto>> Restore(int id)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material is null)
            return NotFound();

        if (!material.IsArchived)
            return await GetById(id);

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            return Problem(detail: "Ошибка получения Id пользователя, пытавшегося восстановить материал из архива.", statusCode: StatusCodes.Status500InternalServerError);

        material.IsArchived = false;
        material.Audit = material.Audit with { EditedByUserId = userId, EditedAt = DateTime.Now };

        await _db.SaveChangesAsync();
        var materialDto = await ToDto(_db.Materials.Where(m => m.Id == material.Id)).SingleAsync();

        return Ok(materialDto);
    }
}