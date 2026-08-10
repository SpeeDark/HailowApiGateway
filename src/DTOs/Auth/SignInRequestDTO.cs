using System.ComponentModel.DataAnnotations;

namespace HailowApiGateway.DTOs.Auth;

public class SignInRequestDto
{
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    public string Password { get; set; } = string.Empty;
}
