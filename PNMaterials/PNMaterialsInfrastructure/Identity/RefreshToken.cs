using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsInfrastructure.Identity;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public int UserId { get; set; }
    public PNUser User { get; set; }
}
