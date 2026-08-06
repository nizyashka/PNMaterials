using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;
using PNMaterialsInfrastructure.Services;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseRequestsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IRequestNumberGenerator _numberGenerator;

    public PurchaseRequestsController(AppDbContext db, IRequestNumberGenerator numberGenerator)
    {
        _db = db;
        _numberGenerator = numberGenerator;
    }

    // Список заявок
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDto>>> GetAll()
    {
        var requests = await QueryWithIncludes()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(requests.Select(ToDto).ToList());
    }

    // Одна заявка
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseRequestDto>> GetById(int id)
    {
        var request = await QueryWithIncludes().FirstOrDefaultAsync(r => r.Id == id);
        if (request is null)
            return NotFound();

        return Ok(ToDto(request));
    }

    // Создать заявку
    [HttpPost]
    public async Task<ActionResult<PurchaseRequestDto>> Create(PurchaseRequestEditDto dto)
    {
        var deliveryDate = dto.DeliveryDate?.Date ?? DateTime.Today.AddMonths(1);

        if (deliveryDate < DateTime.Today)
            return BadRequest("Дата поставки не может быть в прошлом.");

        var error = await ValidateItemsAsync(dto.Items);
        if (error is not null)
            return BadRequest(error);

        var request = new PurchaseRequest
        {
            Number = await _numberGenerator.NextAsync(),
            CreatedAt = DateTime.Now,
            DeliveryDate = deliveryDate,
            Status = RequestStatus.Created,
            Items = dto.Items.Select(i => new PurchaseRequestItem
            {
                MaterialId = i.MaterialId,
                Quantity = i.Quantity,
                PositionText = i.PositionText
            }).ToList()
        };

        _db.PurchaseRequests.Add(request);
        await _db.SaveChangesAsync();

        var created = await QueryWithIncludes().FirstAsync(r => r.Id == request.Id);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, ToDto(created));
    }

    // Изменить заявку (дата поставки и состав позиций)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PurchaseRequestEditDto dto)
    {
        var request = await _db.PurchaseRequests
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null)
            return NotFound();

        var deliveryDate = dto.DeliveryDate?.Date ?? request.DeliveryDate;
        if (deliveryDate < DateTime.Today)
            return BadRequest("Дата поставки не может быть в прошлом.");

        var error = await ValidateItemsAsync(dto.Items);
        if (error is not null)
            return BadRequest(error);

        request.DeliveryDate = deliveryDate;

        request.Items.Clear();
        foreach (var i in dto.Items)
        {
            request.Items.Add(new PurchaseRequestItem
            {
                MaterialId = i.MaterialId,
                Quantity = i.Quantity,
                PositionText = i.PositionText
            });
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Сменить статус
    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<PurchaseRequestDto>> ChangeStatus(int id, RequestStatusChangeDto dto)
    {
        var request = await _db.PurchaseRequests.FindAsync(id);
        if (request is null)
            return NotFound();

        var target = (RequestStatus)dto.StatusId;
        if (!Enum.IsDefined(target))
            return BadRequest("Некорректный статус.");

        if (!RequestStatusRules.CanTransition(request.Status, target))
            return Conflict(
                $"Переход {RequestStatusInfo.GetCode(request.Status)} → " +
                $"{RequestStatusInfo.GetCode(target)} не разрешён.");

        request.Status = target;
        await _db.SaveChangesAsync();

        var updated = await QueryWithIncludes().FirstAsync(r => r.Id == id);
        return Ok(ToDto(updated));
    }

    [HttpGet("statuses")]
    public ActionResult<IEnumerable<RequestStatusDto>> GetStatuses()
    => Ok(Enum.GetValues<RequestStatus>().Select(ToStatusDto).ToList());

    // Вспомогательные методы

    private IQueryable<PurchaseRequest> QueryWithIncludes()
        => _db.PurchaseRequests
            .AsNoTracking()
            .Include(r => r.Items)
                .ThenInclude(i => i.Material)
                    .ThenInclude(m => m.Unit);

    private async Task<string?> ValidateItemsAsync(List<PurchaseRequestItemEditDto> items)
    {
        if (items.Count == 0)
            return "Заявка должна содержать хотя бы одну позицию.";

        var ids = items.Select(i => i.MaterialId).Distinct().ToList();

        var existing = await _db.Materials
            .Where(m => ids.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync();

        var missing = ids.Except(existing).ToList();
        if (missing.Count > 0)
            return $"Материалы не найдены: {string.Join(", ", missing)}.";

        return null;
    }

    private static PurchaseRequestDto ToDto(PurchaseRequest r) => new()
    {
        Id = r.Id,
        Number = r.Number,
        CreatedAt = r.CreatedAt,
        DeliveryDate = r.DeliveryDate,
        Status = ToStatusDto(r.Status),
        AllowedTransitions = RequestStatusRules.AllowedTargets(r.Status)
            .Select(ToStatusDto)
            .ToList(),
        Items = r.Items.Select(i => new PurchaseRequestItemDto
        {
            Id = i.Id,
            MaterialId = i.MaterialId,
            MaterialCode = i.Material.Code,
            MaterialName = i.Material.Name,
            UnitName = i.Material.Unit.Name,
            Quantity = i.Quantity,
            PositionText = i.PositionText
        }).ToList()
    };

    private static RequestStatusDto ToStatusDto(RequestStatus status) => new()
    {
        Id = (int)status,
        Code = RequestStatusInfo.GetCode(status),
        Name = RequestStatusInfo.GetName(status)
    };
}