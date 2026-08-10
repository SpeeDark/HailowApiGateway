using System.ComponentModel.DataAnnotations;

namespace HailowApiGateway.DTOs.Auth;

public class UpdateDeliveryInfoRequestDto
{
    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Street is required")]
    public string Street { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Building is required")]
    public string Building { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Invalid porch value")]
    public int? Porch { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Invalid floor value")]
    public int? Floor { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Invalid flat value")]
    public int? Flat { get; set; }
}