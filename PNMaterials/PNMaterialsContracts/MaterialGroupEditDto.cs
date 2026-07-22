using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class MaterialGroupEditDto
{
    [Required(ErrorMessage = "Наименование группы обязательно")]
    [MaxLength(40, ErrorMessage = "Наименование не длиннее 40 символов")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Описание не длиннее 200 символов")]
    public string? Description { get; set; }
}