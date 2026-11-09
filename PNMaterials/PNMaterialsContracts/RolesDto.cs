using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PNMaterialsContracts;

public class RolesDto
{
    [Required(ErrorMessage = "Выберите хотя бы одну роль.")]
    [MinLength(1, ErrorMessage = "Выберите хотя бы одну роль.")]
    public string[] Roles { get; set; } = [];
}
