using System.ComponentModel.DataAnnotations;

namespace PNMaterialsContracts;

public class CreateUserDto
{
    [Required(ErrorMessage = "Логин обязателен.")]
    [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Пароль обязателен.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Имя обязательно.")]
    [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна.")]
    [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов.")]
    public string LastName { get; set; }

    [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов.")]
    public string? MiddleName { get; set; }

    public List<string> Roles { get; set; }
}
