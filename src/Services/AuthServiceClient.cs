using Grpc.Net.Client;
using HailowApiGateway.Protos;

namespace HailowApiGateway.Services;

public interface IAuthServiceClient
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

public class AuthServiceClient : IAuthServiceClient
{
    private readonly AuthService.AuthServiceClient _client;

    public AuthServiceClient(AuthService.AuthServiceClient client)
    {
        _client = client;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            return await _client.RegisterAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(RegisterAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            return await _client.LoginAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(LoginAsync)}: {ex.Status.Detail}", ex);
        }
    }
}