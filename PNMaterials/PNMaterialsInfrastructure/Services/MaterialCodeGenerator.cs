using Microsoft.EntityFrameworkCore;
using PNMaterialsDomain;

namespace PNMaterialsInfrastructure.Services;

public class MaterialCodeGenerator : IMaterialCodeGenerator
{
    private readonly AppDbContext _db;

    public MaterialCodeGenerator(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string> NextAsync(CancellationToken ct = default)
    {
        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            var conn = _db.Database.GetDbConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT NEXT VALUE FOR MaterialCodeSequence";

            var raw = await cmd.ExecuteScalarAsync(ct);
            var value = Convert.ToInt64(raw);

            return MaterialCodeFormatter.Format(value);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}