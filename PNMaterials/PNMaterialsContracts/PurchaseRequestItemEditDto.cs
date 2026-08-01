using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class PurchaseRequestItemEditDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Не выбран материал")]
    public int MaterialId { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "Количество должно быть от 0,01 до 999999,99")]
    public decimal Quantity { get; set; }

    [MaxLength(200, ErrorMessage = "Текст к позиции не длиннее 200 символов")]
    public string? PositionText { get; set; }
}