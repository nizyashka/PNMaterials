using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class LoginDto
{
    [Required(ErrorMessage = "Логин обязателен")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}
