using HailowApiGateway.Config;
using HailowApiGateway.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HailowApiGateway.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HailowApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configuration
        builder.Services.AddAppConfig();
        
        // Postgres
        builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var config = serviceProvider.GetRequiredService<AppConfig>();
            options.UseNpgsql(config.PostgresConnectionString);
        });
        
        // RabbitMQ
        builder.Services.AddMassTransit(x =>
        {
            // Consumers registration (later)
            // x.AddConsumer<OrderCreatedConsumer>();
            // x.AddConsumer<PaymentProcessedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var config = context.GetRequiredService<AppConfig>();

                cfg.Host(config.RabbitMqHost, "/", host =>
                {
                    host.Username(config.RabbitMqUser);
                    host.Password(config.RabbitMqPass);
                });
            });
            
            // Queue configuration (later)
            // cfg.ReceiveEndpoint("order-created-queue", e =>
            // {
            //     e.ConfigureConsumer<OrderCreatedConsumer>(context);
            // });
        });
        
        builder.Services.AddControllers();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // gRPC Clients
        
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}