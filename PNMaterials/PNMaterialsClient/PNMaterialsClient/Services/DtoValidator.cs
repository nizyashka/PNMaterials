using System.ComponentModel.DataAnnotations;

namespace PNMaterialsClient.Services;

public static class DtoValidator
{
    public static IReadOnlyList<string> Validate(object dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results
            .Select(r => r.ErrorMessage ?? "Некорректные данные.")
            .ToList();
    }
}
