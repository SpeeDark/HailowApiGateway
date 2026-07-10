using Microsoft.AspNetCore.Mvc;
using HailowApiGateway.Protos;
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

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (
            string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password)
            || string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Email, password, name are required");
        }

        var grpcRequest = new RegisterRequest
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name
        };

        try
        {
            var response = await _authClient.RegisterAsync(grpcRequest);

            return Ok(new
            {
                UserId = response.UserId,
                Name = response.Name,
                Email = response.Email
            });;
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message} );
        }
    }


    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Email, password, name are required");
        }

        var grpcRequest = new LoginRequest
        {
            Email = request.Email,
            Password = request.Password
        };

        try
        {
            var response = await _authClient.LoginAsync(grpcRequest);

            return Ok(new
            {
                UserId = response.UserId,
                Email = response.Email,
                Name = response.Name,
                Token = response.Token
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}