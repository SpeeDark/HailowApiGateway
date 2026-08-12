using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using HailowApiGateway.Config;
using HailowApiGateway.Models;

namespace HailowApiGateway.Services;

public interface IJwtValidationService
{
    Task<JwtValidationResult> ValidateTokenAsync(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}

public interface IJwtValidationCustomerService : IJwtValidationService {}
public interface IJwtValidationSellerService : IJwtValidationService {}

public abstract class BaseJwtValidationService : IJwtValidationService
{
    protected readonly AppConfig _config;
    protected readonly IRedisServiceClient _redis;
    protected readonly JwtSecurityTokenHandler _tokenHandler;
    protected readonly string _tokenBlacklistKeyPrefix;
    protected readonly string _tokenValidKeyPrefix;
    protected readonly string _role;
    protected readonly string _signingKey;
    protected readonly string _issuer;
    protected readonly string _audience;
    protected readonly int _cacheDurationMinutes;
    
    protected BaseJwtValidationService(
        AppConfig config,
        IRedisServiceClient redis,
        string role,
        string signingKey,
        string issuer,
        string audience,
        int cacheDurationMinutes = 5,
        string tokenBlacklistKeyPrefix = "token:blacklist",
        string tokenValidKeyPrefix = "token:valid")
    {
        _config = config;
        _redis = redis;
        _role = role;
        _signingKey = signingKey;
        _issuer = issuer;
        _audience = audience;
        _tokenHandler = new JwtSecurityTokenHandler();
        _cacheDurationMinutes = cacheDurationMinutes;
        _tokenBlacklistKeyPrefix = tokenBlacklistKeyPrefix;
        _tokenValidKeyPrefix = tokenValidKeyPrefix;
    }
    
    public virtual async Task<JwtValidationResult> ValidateTokenAsync(string token)
    {
        // var isBlacklisted = await _redis.ExistsAsync($"{_tokenBlacklistKeyPrefix}:{token}");
        // if (isBlacklisted)
        // {
        //     var result = new JwtValidationResult();
        //     result.ErrorMessage = "Token has been revoked";
        //     Console.WriteLine("Token is blacklisted");
        //     return result;
        // }

        var cachedResult = await _redis.GetAsync<JwtValidationResult>($"{_tokenValidKeyPrefix}:{token}");
        if (cachedResult != null)
            return cachedResult;

        var validationResult = GetValidationResult(token);
        if (!validationResult.IsValid)
            return validationResult;
        
        var cacheDuration = TimeSpan.FromMinutes(_cacheDurationMinutes);
        
        await _redis.SetAsync($"{_tokenValidKeyPrefix}:{token}", validationResult, cacheDuration);

        return validationResult;
    }

    protected virtual JwtValidationResult GetValidationResult(string token)
    {
        var result = new JwtValidationResult();

        var principal = Validate(token);
        if (principal == null)
        {
            result.IsValid = false;
            result.ErrorMessage = "Invalid token";
            return result;
        }

        var role = principal.FindFirst(ClaimTypes.Role)?.Value ??
                   principal.FindFirst("role")?.Value ??
                   principal.FindFirst("Role")?.Value;
        var userId = principal.FindFirst("id")?.Value ??
                 principal.FindFirst("ID")?.Value ??
                 principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value ??
                    principal.FindFirst("email")?.Value ??
                    principal.FindFirst("Email")?.Value;
        
        var claims = new Dictionary<string, string>();
        foreach (var claim in principal.Claims)
        {
            if (!claims.ContainsKey(claim.Type))
            {
                claims[claim.Type] = claim.Value;
            }
        }

        result.IsValid = true;
        result.Role = role;
        result.UserId = userId;
        result.Email = email;
        result.Claims = claims;

        Console.WriteLine($"Token validated successfully.\nRole: {role},\nUserId: {userId},\nEmail: {email}");
        return result;
    }

    protected virtual ClaimsPrincipal? Validate(string token)
    {
        try
        {
            var key = Encoding.ASCII.GetBytes(_signingKey);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,//true,
                ValidIssuer = _issuer,
                ValidateAudience = false,//true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "email",
                RoleClaimType = "role"
            };

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParams, out _);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            Console.WriteLine($"Token expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            Console.WriteLine($"Invalid token signature");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating token");
            return null;
        }
    }

    public virtual ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        return Validate(token);
    }
}

public class JwtValidationCustomerService : BaseJwtValidationService, IJwtValidationCustomerService
{
    public JwtValidationCustomerService(
        AppConfig config,
        IRedisServiceClient redis)
        : base(
            config,
            redis,
            role: "customer",
            signingKey: config.JwtAccessSecretCustomer,
            issuer: "hailow-customer",
            audience: "hailow-api",
            cacheDurationMinutes: 10,
            tokenBlacklistKeyPrefix: "token:customer:blacklist",
            tokenValidKeyPrefix: "token:customer:valid")
    {
    }
}

public class JwtValidationSellerService : BaseJwtValidationService, IJwtValidationSellerService
{
    public JwtValidationSellerService(
        AppConfig config,
        IRedisServiceClient redis)
        : base(
            config,
            redis,
            role: "seller",
            signingKey: config.JwtAccessSecretSeller,
            issuer: "hailow-seller",
            audience: "hailow-api",
            cacheDurationMinutes: 10,
            tokenBlacklistKeyPrefix: "token:seller:blacklist",
            tokenValidKeyPrefix: "token:seller:valid")
    {
    }
}
