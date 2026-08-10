using System.ComponentModel.DataAnnotations;

namespace HailowApiGateway.DTOs.Auth;

public class RefreshTokensRequestDto
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}