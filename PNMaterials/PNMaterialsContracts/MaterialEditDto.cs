using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class MaterialEditDto
{
    [Required(ErrorMessage = "Наименование материала обязательно")]
    [MaxLength(40, ErrorMessage = "Наименование не длиннее 40 символов")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Описание не длиннее 200 символов")]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Не выбрана единица измерения")]
    public int UnitId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Не выбрана группа материалов")]
    public int GroupId { get; set; }
}