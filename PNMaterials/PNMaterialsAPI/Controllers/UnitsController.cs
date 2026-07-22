using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PNMaterialsContracts;
using PNMaterialsInfrastructure;

namespace PNMaterialsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly AppDbContext _db;

    public UnitsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
    {
        var units = await _db.Units
            .OrderBy(u => u.Id)
            .Select(u => new UnitDto { Id = u.Id, Name = u.Name })
            .ToListAsync();

        return Ok(units);
    }
}