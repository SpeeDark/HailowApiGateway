using HailowApiGateway.Config;
using Microsoft.Extensions.DependencyInjection;

namespace HailowApiGateway.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Register AppConfig in DI container
    /// </summary>
    public static IServiceCollection AddAppConfig(this IServiceCollection services)
    {
        var config = ConfigLoader.Load();
        services.AddSingleton(config);
        return services;
    }
}