using HailowApiGateway.DTOs.Types;

namespace HailowApiGateway.DTOs;

public class CreateRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
}

public class DeleteRequestDto
{
    public string Name { get; set; } = string.Empty;
}

public class CreateResponseDto
{
    public string Name { get; set; } = string.Empty;
}

public class DeleteResponseDto
{
    public string Name { get; set; } = string.Empty;
}