using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class PasswordEditDto
{
    [Required(ErrorMessage = "Новый пароль не может быть пустым.")]
    public string NewPassword { get; set; } = string.Empty;
}
