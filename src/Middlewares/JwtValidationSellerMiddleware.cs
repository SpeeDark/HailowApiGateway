using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using HailowApiGateway.Services;
using Microsoft.AspNetCore.Http;

namespace HailowApiGateway.Middlewares;

public class JwtValidationSellerMiddleware : IMiddleware
{
    private readonly IJwtValidationSellerService _jwtValidationService;

    private static readonly string[] PublicPaths = 
    {
        "/health",
        "/api/auth/login",
        "/api/auth/register",
        "/swagger",
        "/openapi"
    };
    
    public JwtValidationSellerMiddleware(IJwtValidationSellerService jwtValidationService)
    {
        _jwtValidationService = jwtValidationService;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsPublicPath(path))
        {
            await next(context);
            return;
        }

        var token = ExtractToken(context);
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"No token provided for path: {path}");
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 401;
            
            var error = new { error = "Authorization token required" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            return;
        }

        try
        {
            var validationResult = await _jwtValidationService.ValidateTokenAsync(token);

            if (!validationResult.IsValid)
            {
                Console.WriteLine($"Seller token validation failed: {validationResult.ErrorMessage}");
                
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var error = new { error = validationResult.ErrorMessage ?? "Unauthorized" };
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
                return;
            }

            Console.WriteLine($"Seller token validated. UserId: {validationResult.UserId}");

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, validationResult.UserId),
                new Claim(ClaimTypes.Email, validationResult.Email),
                new Claim(ClaimTypes.Role, validationResult.Role),
                new Claim("id", validationResult.UserId),
                new Claim("email", validationResult.Email),
                new Claim("role", validationResult.Role),
            }, "JWT");

            context.User = new ClaimsPrincipal(identity);

            context.Items["UserId"] = validationResult.UserId;
            context.Items["UserRole"] = validationResult.Role;
            context.Items["UserEmail"] = validationResult.Email;

            await next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seller JWT validation: {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

    private bool IsPublicPath(string path)
    {
        return PublicPaths.Any(p => 
            path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private string? ExtractToken(HttpContext context)
    {
        // Authorization header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && 
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Query param
        var queryToken = context.Request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(queryToken))
        {
            return queryToken;
        }

        // Cookie
        var cookieToken = context.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(cookieToken))
        {
            return cookieToken;
        }

        return null;
    }
}