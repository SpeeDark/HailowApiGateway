using System;
using DotNetEnv;

namespace HailowApiGateway.Config;

public static class ConfigLoader
{
    public static AppConfig Load()
    {
        Env.Load();

        int authServicePort = GetEnvInt("AUTH_SERVICE_PORT");
        int productServicePort = GetEnvInt("PRODUCT_SERVICE_PORT");
        int cartServicePort = GetEnvInt("CART_SERVICE_PORT");
        int paymentServicePort = GetEnvInt("PAYMENT_SERVICE_PORT");
        int orderServicePort = GetEnvInt("ORDER_SERVICE_PORT");
        
        return new AppConfig
        {
            // JWT
            JwtAccessSecret = GetEnv("JWT_ACCESS_KEY"),
            JwtRefreshSecret = GetEnv("JWT_REFRESH_KEY"),
            
            // PostgreSQL
            PostgresHost = GetEnv("POSTGRES_HOST"),
            PostgresPort = GetEnvInt("POSTGRES_PORT"),
            PostgresUser = GetEnv("POSTGRES_USERNAME"),
            PostgresPassword = GetEnv("POSTGRES_PASSWORD"),
            PostgresDb = GetEnv("POSTGRES_DATABASE"),

            // RabbitMQ
            RabbitMqHost = GetEnv("RABBITMQ_HOST"),
            RabbitMqPort = GetEnvInt("RABBITMQ_PORT"),
            RabbitMqUser = GetEnv("RABBITMQ_USERNAME"),
            RabbitMqPassword = GetEnv("RABBITMQ_PASSWORD"),

            // Ports
            AuthServicePort = authServicePort,
            ProductServicePort = productServicePort,
            CartServicePort = cartServicePort,
            PaymentServicePort = paymentServicePort,
            OrderServicePort = orderServicePort,
            
            // Services
            AuthServiceUrl = $"{GetEnv("AUTH_SERVICE_URL")}:{authServicePort}",
            ProductServiceUrl = $"{GetEnv("PRODUCT_SERVICE_URL")}:{productServicePort}",
            CartServiceUrl = $"{GetEnv("CART_SERVICE_URL")}:{cartServicePort}",
            PaymentServiceUrl = $"{GetEnv("PAYMENT_SERVICE_URL")}:{paymentServicePort}",
            
            // S3
            S3Endpoint = GetEnv("S3_ENDPOINT"),
            S3AccessKey = GetEnv("S3_ACCESS_KEY"),
            S3SecretKey = GetEnv("S3_SECRET_KEY"),
            S3Bucket = GetEnv("S3_BUCKET"),
        };
    }

    private static string GetEnv(string key, string defaultValue = "")
        => Environment.GetEnvironmentVariable(key) ?? defaultValue;

    private static int GetEnvInt(string key, int defaultValue = 0)
        => int.TryParse(Environment.GetEnvironmentVariable(key), out var value) ? value : defaultValue;

    private static bool GetEnvBool(string key, bool defaultValue = false)
        => bool.TryParse(Environment.GetEnvironmentVariable(key), out var value) ? value : defaultValue;
}