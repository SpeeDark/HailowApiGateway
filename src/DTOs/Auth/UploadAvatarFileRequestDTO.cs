using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HailowApiGateway.DTOs.Auth;

public class UploadAvatarFileRequestDto
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Avatar file is required")]
    public IFormFile AvatarFile { get; set; } = null!;
}