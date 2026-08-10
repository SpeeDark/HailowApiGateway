using System.ComponentModel.DataAnnotations;

namespace HailowApiGateway.DTOs.Auth;

public class SignUpRequestDto
{
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Invalid porch value")]
    public int? Porch { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Invalid floor value")]
    public int? Floor { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Invalid flat value")]
    public int? Flat { get; set; }
}