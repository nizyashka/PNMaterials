using Microsoft.EntityFrameworkCore;
using PNMaterialsDomain;

namespace PNMaterialsInfrastructure.Services;

public class RequestNumberGenerator : IRequestNumberGenerator
{
    private readonly AppDbContext _db;

    public RequestNumberGenerator(AppDbContext db)
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
            cmd.CommandText = "SELECT NEXT VALUE FOR RequestNumberSequence";

            var raw = await cmd.ExecuteScalarAsync(ct);
            var value = Convert.ToInt64(raw);

            return RequestNumberFormatter.Format(value);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}