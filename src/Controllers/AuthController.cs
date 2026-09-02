using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using HailowApiGateway.Attributes;
using Microsoft.AspNetCore.Mvc;
using HailowApiGateway.Protos.AuthService;
using HailowApiGateway.Services;
using HailowApiGateway.DTOs.Auth;

namespace HailowApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServiceClient _authClient;

    public AuthController(IAuthServiceClient authClient)
    {
        _authClient = authClient;
    }

    [HttpPost("signup-customer")]
    public async Task<IActionResult> SignUpCustomer([FromBody] SignUpRequestDto request)
    {
        var grpcRequest = new CustomerSignUpRequest
        {
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            City = request.City ?? string.Empty,
            Street = request.Street ?? string.Empty,
            Building = request.Building ?? string.Empty,
        };

        if (request.Porch.HasValue)  grpcRequest.Porch = request.Porch.Value;
        if (request.Floor.HasValue)  grpcRequest.Floor = request.Floor.Value;
        if (request.Flat.HasValue) grpcRequest.Flat = request.Flat.Value;

        try
        {
            var response = await _authClient.SignUpCustomerAsync(grpcRequest);

            return Ok(new
            {
                Id = response.User.Id,
                FirstName = response.User.FirstName,
                LastName = response.User.LastName,
                Email = response.User.Email,
                Phone = response.User.PhoneNumber,
                Role = response.User.Role,
                // CreatedAt = response.User.CreatedAt,
                // UpdatedAt = response.User.UpdatedAt,
            });;
        }
        catch (Exception ex)
        {
            throw;
            return BadRequest(new { Error = ex.Message} );
        }
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequestDto request)
    {
        var grpcRequest = new SignInRequest
        {
            Email = request.Email,
            Password = request.Password
        };

        try
        {
            var response = await _authClient.SignInAsync(grpcRequest);

            return Ok(new
            {
                AccessToken = response.Tokens.AccessToken,
                RefreshToken = response.Tokens.RefreshToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensRequestDto request)
    {
        var grpcRequest = new RefreshTokensRequest
        {
            RefreshToken = request.RefreshToken
        };

        try
        {
            var response = await _authClient.RefreshTokensAsync(grpcRequest);

            return Ok(new
            {
                AccessToken = response.AccessToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var grpcRequest = new LogoutRequest { };

        try
        {
            var response = await _authClient.LogoutAsync(grpcRequest);

            if (response.Success)
            {
                return Ok(new { Message = "Logged out successfully" });
            }
            
            return BadRequest(new { Error = "Logout failed" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }


    #region Profile management
    
    [HttpGet("profile/{id}")]
    [Ownership]
    public async Task<IActionResult> GetProfile(string id)
    {
        var grpcRequest = new GetProfileRequest
        {
            Id = id
        };

        try
        {
            var response = await _authClient.GetProfileAsync(grpcRequest);

            return Ok(new
            {
                Id = response.User.Id,
                FirstName = response.User.FirstName,
                LastName = response.User.LastName,
                Email = response.User.Email,
                PhoneNumber = response.User.PhoneNumber,
                AvatarUrl = response.User.AvatarUrl,
                City = response.User.City,
                Street = response.User.Street,
                Building = response.User.Building,
                Porch = response.User.HasPorch ? response.User.Porch : (int?)null,
                Floor = response.User.HasFloor ? response.User.Floor : (int?)null,
                Flat = response.User.HasFlat ? response.User.Flat : (int?)null,
                Role = response.User.Role,
                CreatedAt = response.User.CreatedAt,
                UpdatedAt = response.User.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("profile/{id}")]
    [Ownership]
    public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfileRequestDto request)
    {
        var grpcRequest = new UpdateProfileRequest
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        try
        {
            var response = await _authClient.UpdateProfileAsync(grpcRequest);

            return Ok(new
            {
                Id = response.User.Id,
                FirstName = response.User.FirstName,
                LastName = response.User.LastName,
                Email = response.User.Email,
                PhoneNumber = response.User.PhoneNumber,
                Role = response.User.Role,
                CreatedAt = response.User.CreatedAt,
                UpdatedAt = response.User.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("profile/{id}/delivery")]
    [Ownership]
    public async Task<IActionResult> UpdateDeliveryInfo(string id, [FromBody] UpdateDeliveryInfoRequestDto request)
    {
        var grpcRequest = new UpdateDeliveryInfoRequest
        {
            Id = id,
            City = request.City,
            Street = request.Street,
            Building = request.Building,
        };

        if (request.Porch.HasValue) grpcRequest.Porch = request.Porch.Value;
        if (request.Floor.HasValue) grpcRequest.Floor = request.Floor.Value;
        if (request.Flat.HasValue) grpcRequest.Flat = request.Flat.Value;

        try
        {
            var response = await _authClient.UpdateDeliveryInfoAsync(grpcRequest);

            return Ok(new
            {
                Id = response.User.Id,
                FirstName = response.User.FirstName,
                LastName = response.User.LastName,
                Email = response.User.Email,
                PhoneNumber = response.User.PhoneNumber,
                AvatarUrl = response.User.AvatarUrl,
                City = response.User.City,
                Street = response.User.Street,
                Building = response.User.Building,
                Porch = response.User.HasPorch ? response.User.Porch : (int?)null,
                Floor = response.User.HasFloor ? response.User.Floor : (int?)null,
                Flat = response.User.HasFlat ? response.User.Flat : (int?)null,
                Role = response.User.Role,
                CreatedAt = response.User.CreatedAt,
                UpdatedAt = response.User.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("profile/{id}/avatar/upload")]
    [Ownership]
    public async Task<IActionResult> UploadAvatarFromFile(string id, [FromForm] UploadAvatarFileRequestDto request)
    {
        byte[] avatarData;
        using (var ms = new MemoryStream())
        {
            await request.AvatarFile.CopyToAsync(ms);
            avatarData = ms.ToArray();
        }

        var grpcRequest = new UploadAvatarRequest
        {
            UserId = id,
            AvatarImage = Google.Protobuf.ByteString.CopyFrom(avatarData)
        };

        try
        {
            var response = await _authClient.UploadAvatarAsync(grpcRequest);

            return Ok(new
            {
                AvatarUrl = response.AvatarUrl
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // [HttpPut("profile/password")]
    // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    // {
    //     var grpcRequest = new ResetPasswordRequest
    //     {
    //         Id = request.Id,
    //         Email = request.Email,
    //         OldPassword = request.OldPassword,
    //         NewPassword = request.NewPassword
    //     };
    //
    //     try
    //     {
    //         var response = await _authClient.ResetPasswordAsync(grpcRequest);
    //
    //         return Ok(new
    //         {
    //             Success = response.Success,
    //             Message = response.Success ? "Password updated successfully" : "Password update failed"
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { Error = ex.Message });
    //     }
    // }

    // [HttpDelete("profile/{id}")]
    // public async Task<IActionResult> DeleteAccount(string id)
    // {
    //     var grpcRequest = new DeleteAccountRequest
    //     {
    //         Id = id
    //     };
    //
    //     try
    //     {
    //         var response = await _authClient.DeleteAccountAsync(grpcRequest);
    //
    //         if (response.Success)
    //         {
    //             return Ok(new { Message = "Account deleted successfully" });
    //         }
    //         
    //         return BadRequest(new { Error = "Account deletion failed" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { Error = ex.Message });
    //     }
    // }
    
    #endregion
}