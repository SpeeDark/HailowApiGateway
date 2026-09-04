using System;
using System.Linq;
using System.Security.Claims;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace HailowApiGateway.Interceptors;

public class MetadataInterceptor : Interceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public MetadataInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = httpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        var token = httpContext?.Request.Headers[HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
        var refreshToken = httpContext?.Request.Headers["refreshToken"].FirstOrDefault();
        
        Console.WriteLine("Interceptor");
        Console.WriteLine($"userId: {userId}");
        Console.WriteLine($"role: {role}");
        Console.WriteLine($"token: {token}");
        Console.WriteLine($"refreshToken: {refreshToken}");
        
        var metadata = new Metadata();
        
        if (!string.IsNullOrEmpty(userId))
            metadata.Add("user-id", userId);
        if (!string.IsNullOrEmpty(role))
            metadata.Add("user-role", role);
        if (!string.IsNullOrEmpty(token))
            metadata.Add("authorization", $"Bearer {token}");
        if (!string.IsNullOrEmpty(refreshToken))
            metadata.Add("refresh-token", refreshToken);
        if (httpContext?.TraceIdentifier != null)
            metadata.Add("request-id", httpContext.TraceIdentifier);
        
        var newOptions = context.Options.WithHeaders(metadata);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            newOptions);
        
        return continuation(request, newContext);
    }
}