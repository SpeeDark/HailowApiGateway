using System.ComponentModel.DataAnnotations;

namespace HailowApiGateway.DTOs.Auth;

public class LogoutRequestDto
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}