namespace PNMaterialsInfrastructure.Services;

public interface IMaterialCodeGenerator
{
    Task<string> NextAsync(CancellationToken ct = default);
}