using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Services;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMaterialCodeGenerator _codeGenerator;

    public MaterialsController(AppDbContext db, IMaterialCodeGenerator codeGenerator)
    {
        _db = db;
        _codeGenerator = codeGenerator;
    }

    // Список всех материалов
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll()
    {
        var materials = await _db.Materials
            .OrderBy(m => m.Code)
            .Select(m => new MaterialDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Description = m.Description,
                UnitId = m.UnitId,
                UnitName = m.Unit.Name,
                GroupId = m.GroupId,
                GroupName = m.Group.Name
            })
            .ToListAsync();

        return Ok(materials);
    }

    // Один материал по id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaterialDto>> GetById(int id)
    {
        var material = await _db.Materials
            .Where(m => m.Id == id)
            .Select(m => new MaterialDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Description = m.Description,
                UnitId = m.UnitId,
                UnitName = m.Unit.Name,
                GroupId = m.GroupId,
                GroupName = m.Group.Name
            })
            .FirstOrDefaultAsync();

        if (material is null)
            return NotFound();

        return Ok(material);
    }

    // Создать
    [HttpPost]
    public async Task<ActionResult<MaterialDto>> Create(MaterialEditDto dto)
    {
        var error = await ValidateReferencesAsync(dto);
        if (error is not null)
            return BadRequest(error);

        var material = new Material
        {
            Code = await _codeGenerator.NextAsync(),
            Name = dto.Name,
            Description = dto.Description,
            UnitId = dto.UnitId,
            GroupId = dto.GroupId
        };

        _db.Materials.Add(material);
        await _db.SaveChangesAsync();

        var result = await ToDtoAsync(material.Id);
        return CreatedAtAction(nameof(GetById), new { id = material.Id }, result);
    }

    // Изменить
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MaterialEditDto dto)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material is null)
            return NotFound();

        var error = await ValidateReferencesAsync(dto);
        if (error is not null)
            return BadRequest(error);

        material.Name = dto.Name;
        material.Description = dto.Description;
        material.UnitId = dto.UnitId;
        material.GroupId = dto.GroupId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Удалить
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var material = await _db.Materials.FindAsync(id);
        if (material is null)
            return NotFound();

        var usedInRequests = await _db.PurchaseRequestItems.AnyAsync(i => i.MaterialId == id);
        if (usedInRequests)
            return Conflict("Нельзя удалить материал: он используется в заявках.");

        _db.Materials.Remove(material);
        await _db.SaveChangesAsync();
        return NoContent();
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

    // Перечитать материал вместе с названиями единицы и группы
    private async Task<MaterialDto> ToDtoAsync(int id)
    {
        return await _db.Materials
            .Where(m => m.Id == id)
            .Select(m => new MaterialDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Description = m.Description,
                UnitId = m.UnitId,
                UnitName = m.Unit.Name,
                GroupId = m.GroupId,
                GroupName = m.Group.Name
            })
            .FirstAsync();
    }
}