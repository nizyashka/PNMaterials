using System;
using System.Collections.Generic;
using System.Text;

namespace PNMaterialsContracts;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTimeOffset AccessTokenExpiresAt { get; set; }
}
