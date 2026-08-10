using System.Collections.Generic;

namespace HailowApiGateway.Models;

public class JwtValidationResult
{
    public bool IsValid { get; set; }
    public string Role { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
    public string? ErrorMessage { get; set; }
}