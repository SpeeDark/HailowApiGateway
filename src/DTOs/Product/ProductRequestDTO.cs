namespace HailowApiGateway.DTOs.Product;

public class CreateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
}