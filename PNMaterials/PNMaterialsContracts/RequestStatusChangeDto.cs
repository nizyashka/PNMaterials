using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class RequestStatusChangeDto
{
    [Range(1, 4, ErrorMessage = "Некорректный статус")]
    public int StatusId { get; set; }
}