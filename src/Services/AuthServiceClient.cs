using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using HailowApiGateway.Protos.AuthService;

namespace HailowApiGateway.Services;

public interface IAuthServiceClient
{
    Task<SignUpResponse> SignUpAsync(SignUpRequest request);
    Task<SignInResponse> SignInAsync(SignInRequest request);
}

public class AuthServiceClient : IAuthServiceClient
{
    private readonly AuthService.AuthServiceClient _client;

    public AuthServiceClient(AuthService.AuthServiceClient client)
    {
        _client = client;
    }

    public async Task<SignUpResponse> SignUpAsync(SignUpRequest request)
    {
        try
        {
            return await _client.SignUpAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(SignUpAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<SignInResponse> SignInAsync(SignInRequest request)
    {
        try
        {
            return await _client.SignInAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(SignInAsync)}: {ex.Status.Detail}", ex);
        }
    }
}