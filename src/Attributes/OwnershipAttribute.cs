using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HailowApiGateway.Attributes;

/// <summary>
/// Verify the user has access only to their own resources
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class OwnershipAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _idParameterName;

    public OwnershipAttribute(string idParameterName = "id")
    {
        _idParameterName = idParameterName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var id = context.RouteData.Values[_idParameterName]?.ToString();
        
        if (string.IsNullOrEmpty(id))
        {
            var queryId = context.HttpContext.Request.Query[_idParameterName].FirstOrDefault();
            if (!string.IsNullOrEmpty(queryId))
            {
                id = queryId;
            }
        }

        if (string.IsNullOrEmpty(id))
        {
            context.Result = new BadRequestObjectResult(new { Error = "Resource ID is required" });
            return;
        }

        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (userId != id)
        {
            context.Result = new ForbidResult("You can only access your own resources");
        }
    }
}