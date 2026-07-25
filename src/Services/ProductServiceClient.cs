using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using HailowApiGateway.Protos.ProductService;

namespace HailowApiGateway.Services;

public interface IProductServiceClient
{
    Task<CreateResponse> CreateAsync(CreateRequest request);
    Task<DeleteResponse> DeleteAsync(DeleteRequest request);
}

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductService.ProductServiceClient _client;
    
    public ProductServiceClient(ProductService.ProductServiceClient client)
    {
        _client = client;
    }

    public async Task<CreateResponse> CreateAsync(CreateRequest request)
    {
        try
        {
            return await _client.CreateAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(CreateAsync)}: {ex.Status.Detail}", ex);
        }
    }

    public async Task<DeleteResponse> DeleteAsync(DeleteRequest request)
    {
        try
        {
            return await _client.DeleteAsync(request);
        }
        catch(Grpc.Core.RpcException ex)
        {
            throw new Exception($"Exception of {nameof(DeleteAsync)}: {ex.Status.Detail}", ex);
        }
    }
}