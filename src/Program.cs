using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using HailowApiGateway.Services;
using HailowApiGateway.Config;
using HailowApiGateway.Database;
using HailowApiGateway.Middlewares;
using HailowApiGateway.Protos.AuthService;
using HailowApiGateway.Protos.ProductService;

namespace HailowApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configuration
        var config = ConfigLoader.Load();
        builder.Services.AddSingleton(config);
        
        // Postgres
        builder.Services.AddDbContext<AppDbContext>((options) =>
        {
            options.UseNpgsql(config.PostgresConnectionString);
        });
        
        // Redis
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var cfg = ConfigurationOptions.Parse(
                config.RedisConnectionString
            );
            cfg.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(cfg);
        });
        
        // RabbitMQ
        builder.Services.AddMassTransit(x =>
        {
            // Consumers registration (later)
            // x.AddConsumer<OrderCreatedConsumer>();
            // x.AddConsumer<PaymentProcessedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config.RabbitMqHost, "/", host =>
                {
                    host.Username(config.RabbitMqUser);
                    host.Password(config.RabbitMqPassword);
                });
            });
            
            // Queue configuration (later)
            // cfg.ReceiveEndpoint("order-created-queue", e =>
            // {
            //     e.ConfigureConsumer<OrderCreatedConsumer>(context);
            // });
        });
        
        // gRPC Clients
        builder.Services.AddGrpcClient<AuthService.AuthServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(config.AuthServiceUrl);
        });
        builder.Services.AddGrpcClient<ProductService.ProductServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(config.ProductServiceUrl);
        });
        
        builder.Services.AddScoped<IAuthServiceClient, AuthServiceClient>();
        builder.Services.AddScoped<IProductServiceClient, ProductServiceClient>();
        builder.Services.AddScoped<IRedisServiceClient, RedisServiceClient>();
        builder.Services.AddScoped<IJwtValidationCustomerService, JwtValidationCustomerService>();
        builder.Services.AddScoped<IJwtValidationSellerService, JwtValidationSellerService>();
        builder.Services.AddScoped<JwtValidationCustomerMiddleware>();
        builder.Services.AddScoped<JwtValidationSellerMiddleware>();
        
        builder.Services.AddControllers();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        // Seller Middleware
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/seller") ||
                       context.Request.Path.StartsWithSegments("/product"),
            appBuilder =>
            {
                appBuilder.UseMiddleware<JwtValidationSellerMiddleware>();
            }
        );

        // Customer Middleware
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/cart") ||
                       context.Request.Path.StartsWithSegments("/order"),
            appBuilder =>
            {
                appBuilder.UseMiddleware<JwtValidationCustomerMiddleware>();
            }
        );
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}