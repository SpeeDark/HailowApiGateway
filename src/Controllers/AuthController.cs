using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HailowApiGateway.Protos.AuthService;
using HailowApiGateway.Services;
using HailowApiGateway.DTOs;

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

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequestDto request)
    {
        if (
            string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password)
            || string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Email, password, name are required");
        }

        var grpcRequest = new SignUpRequest
        {
            Email = request.Email,
            Password = request.Password
        };

        try
        {
            var response = await _authClient.SignUpAsync(grpcRequest);

            return Ok(new
            {
                UserId = response.User.Id,
                Name = $"{response.User.FirstName} {response.User.LastName} ",
                Email = response.User.Email,
                Role = response.User.Role
            });;
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message} );
        }
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Email, password, name are required");
        }

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
                UserId = response.User.Id,
                Name = $"{response.User.FirstName} {response.User.LastName} ",
                Email = response.User.Email,
                Role = response.User.Role,
                AccessToken = response.Tokens.AccessToken,
                RefreshToken = response.Tokens.RefreshToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}