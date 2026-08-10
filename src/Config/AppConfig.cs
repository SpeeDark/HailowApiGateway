namespace HailowApiGateway.Config;

public class AppConfig
{
    // Jwt
    public string JwtRefreshSecretCustomer { get; set; } = string.Empty;
    public string JwtAccessSecretCustomer { get; set; } = string.Empty;
    public string JwtRefreshSecretSeller { get; set; } = string.Empty;
    public string JwtAccessSecretSeller { get; set; } = string.Empty;

    
    // PostgreSQL
    public string PostgresHost { get; set; } = string.Empty;
    public int PostgresPort { get; set; }
    public string PostgresUser { get; set; } = string.Empty;
    public string PostgresPassword { get; set; } = string.Empty;
    public string PostgresDb { get; set; } = string.Empty;

    public string PostgresConnectionString =>
        $"Host={PostgresHost};Port={PostgresPort};Database={PostgresDb};Username={PostgresUser};Password={PostgresPassword}";
    
    // Redis
    public string RedisHost { get; set; } = string.Empty;
    public int RedisPort { get; set; }
    
    public string RedisConnectionString => $"{RedisHost}:{RedisPort}";
    
    // RabbitMQ
    public string RabbitMqHost { get; set; } = string.Empty;
    public int RabbitMqPort { get; set; }
    public string RabbitMqUser { get; set; } = string.Empty;
    public string RabbitMqPassword { get; set; } = string.Empty;

    public string RabbitMqConnectionString =>
        $"amqp://{RabbitMqUser}:{RabbitMqPassword}@{RabbitMqHost}:{RabbitMqPort}";

    // gRPC service ports
    public int AuthServicePort { get; set; }
    public int ProductServicePort { get; set; }
    public int CartServicePort { get; set; }
    public int PaymentServicePort { get; set; }
    public int OrderServicePort { get; set; }
    
    // gRPC Services
    public string AuthServiceUrl { get; set; } = string.Empty;
    public string ProductServiceUrl { get; set; } = string.Empty;
    public string CartServiceUrl { get; set; } = string.Empty;
    public string PaymentServiceUrl { get; set; } = string.Empty;

    // S3
    public string S3Endpoint { get; set; } = string.Empty;
    public string S3AccessKey { get; set; } = string.Empty;
    public string S3SecretKey { get; set; } = string.Empty;
    public string S3Bucket { get; set; } = string.Empty;
}