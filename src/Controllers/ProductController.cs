using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HailowApiGateway.Protos.ProductService;
using HailowApiGateway.Services;
using HailowApiGateway.DTOs;
using HailowApiGateway.Protos.ProductService.Types;

namespace HailowApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductServiceClient _productClient;
    
    public ProductController(IProductServiceClient productClient)
    {
        _productClient = productClient;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.CategoryId))
        {
            return BadRequest("Necessary params are required");
        }

        var grpcRequest = new CreateRequest
        {
            Product = new ProductDraft
            {
                Name = request.Name,
                Category = new Category { Id = request.CategoryId, Name = request.Name },
            }
        };

        try
        {
            var response = await _productClient.CreateAsync(grpcRequest);

            return Ok(new
            {
                Id = response.Product.Id,
                Name = response.Product.Name,
                Category = response.Product.Category
            });;
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message} );
        }
    }
}