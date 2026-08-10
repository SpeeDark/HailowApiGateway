using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using HailowApiGateway.Protos.AuthService;

namespace HailowApiGateway.Services;

public interface IAuthServiceClient
{
    // Auth
    Task<CustomerSignUpResponse> SignUpCustomerAsync(CustomerSignUpRequest request);
    Task<SignInResponse> SignInAsync(SignInRequest request);
    Task<RefreshTokensResponse> RefreshTokensAsync(RefreshTokensRequest request);
    Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    
    // Profile Management
    Task<UploadAvatarResponse> UploadAvatarAsync(UploadAvatarRequest request);
    Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request);
    Task<UpdateDeliveryInfoResponse> UpdateDeliveryInfoAsync(UpdateDeliveryInfoRequest request);
    Task<GetProfileResponse> GetProfileAsync(GetProfileRequest request);
    
    // Account Management
    // Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
}

public class AuthServiceClient : IAuthServiceClient
{
    private readonly AuthService.AuthServiceClient _client;

    public AuthServiceClient(AuthService.AuthServiceClient client)
    {
        _client = client;
    }

    public async Task<CustomerSignUpResponse> SignUpCustomerAsync(CustomerSignUpRequest request)
    {
        try
        {
            return await _client.CustomerSignUpAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(SignUpCustomerAsync)}: {ex.Status.Detail}", ex);
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

    public async Task<RefreshTokensResponse> RefreshTokensAsync(RefreshTokensRequest request)
    {
        try
        {
            return await _client.RefreshTokensAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(RefreshTokensAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
    {
        try
        {
            return await _client.LogoutAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(LogoutAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<UploadAvatarResponse> UploadAvatarAsync(UploadAvatarRequest request)
    {
        try
        {
            return await _client.UploadAvatarAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(UploadAvatarAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<UpdateProfileResponse> UpdateProfileAsync(UpdateProfileRequest request)
    {
        try
        {
            return await _client.UpdateProfileAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(UpdateProfileAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<UpdateDeliveryInfoResponse> UpdateDeliveryInfoAsync(UpdateDeliveryInfoRequest request)
    {
        try
        {
            return await _client.UpdateDeliveryInfoAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(UpdateDeliveryInfoAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<GetProfileResponse> GetProfileAsync(GetProfileRequest request)
    {
        try
        {
            return await _client.GetProfileAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(GetProfileAsync)}: {ex.Status.Detail}", ex);
        }
    }
    
    // public async Task<UpdatePasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
    // {
    //     try
    //     {
    //         return await _client.ResetPasswordAsync(request);
    //     }
    //     catch (Grpc.Core.RpcException ex)
    //     {
    //         throw new Exception($"Exception of {nameof(ResetPasswordAsync)}: {ex.Status.Detail}", ex);
    //     }
    // }
}