using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsDomain;
using PNMaterialsDomain.Entities;
using PNMaterialsInfrastructure;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    // Отчёт по заявкам на закупку
    [HttpPost("purchase-requests")]
    public async Task<ActionResult<IEnumerable<PurchaseRequestReportRowDto>>> PurchaseRequests(
        PurchaseRequestReportFilterDto filter)
    {
        IQueryable<PurchaseRequest> query = _db.PurchaseRequests.AsNoTracking();

        // Код заявки — точное совпадение, множественный выбор
        var numbers = filter.RequestNumbers
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .ToList();

        if (numbers.Count > 0)
            query = query.Where(r => numbers.Contains(r.Number));

        // Код материала — точное совпадение, множественный выбор
        var codes = filter.MaterialCodes
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .ToList();

        if (codes.Count > 0)
            query = query.Where(r => r.Items.Any(i => codes.Contains(i.Material.Code)));

        // Наименование материала — маска через *
        if (!string.IsNullOrWhiteSpace(filter.MaterialName))
        {
            var pattern = SearchMaskConverter.ToLikePattern(filter.MaterialName);

            query = query.Where(r => r.Items.Any(i =>
                EF.Functions.Like(i.Material.Name, pattern, SearchMaskConverter.EscapeCharacter)));
        }

        // Дата поставки «с» … «по»
        if (filter.DeliveryDateFrom.HasValue)
        {
            var from = filter.DeliveryDateFrom.Value.Date;
            query = query.Where(r => r.DeliveryDate >= from);
        }

        if (filter.DeliveryDateTo.HasValue)
        {
            var to = filter.DeliveryDateTo.Value.Date;
            query = query.Where(r => r.DeliveryDate <= to);
        }

        // Статус — с возможностью условия «не равно»
        var statuses = filter.StatusIds
            .Where(id => Enum.IsDefined((RequestStatus)id))
            .Select(id => (RequestStatus)id)
            .Distinct()
            .ToList();

        if (statuses.Count > 0)
        {
            query = filter.StatusNotEqual
                ? query.Where(r => !statuses.Contains(r.Status))
                : query.Where(r => statuses.Contains(r.Status));
        }

        var rows = await query
            .OrderBy(r => r.Number)
            .Select(r => new { r.Id, r.Number, r.DeliveryDate, r.Status })
            .ToListAsync();

        var result = rows.Select(r => new PurchaseRequestReportRowDto
        {
            Id = r.Id,
            Number = r.Number,
            DeliveryDate = r.DeliveryDate,
            Status = new RequestStatusDto
            {
                Id = (int)r.Status,
                Code = RequestStatusInfo.GetCode(r.Status),
                Name = RequestStatusInfo.GetName(r.Status)
            }
        }).ToList();

        return Ok(result);
    }
}