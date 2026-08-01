namespace PNMaterialsInfrastructure.Services;

public interface IRequestNumberGenerator
{
    Task<string> NextAsync(CancellationToken ct = default);
}