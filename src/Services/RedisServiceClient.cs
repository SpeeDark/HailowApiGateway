using System;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace HailowApiGateway.Services;

public interface IRedisServiceClient
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task<T?> GetValueAsync<T>(string key) where T : struct;
    Task<string?> GetStringAsync(string key);
    
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null) where T : struct;
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
    
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
}

public class RedisServiceClient : IRedisServiceClient
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    
    public RedisServiceClient(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;
        
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch
        {
            Console.WriteLine($"Error getting key {key} from Redis");
            return null;
        }
    }

    public async Task<T?> GetValueAsync<T>(string key) where T : struct
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;
        
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
        catch
        {
            Console.WriteLine($"Error getting key {key} from Redis");
            return null;
        }
    }

    public async Task<string?> GetStringAsync(string key)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;

            return value.ToString();
        }
        catch
        {
            Console.WriteLine($"Error getting key {key} from Redis");
            return null;
        }
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting string key {key} in Redis");
        }
    }

    public async Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null) where T : struct
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting string key {key} in Redis");
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        try
        {
            await _db.StringSetAsync(key, value, expiry ?? TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting string key {key} in Redis");
        }
    }
    
    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            return await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting key {key} in Redis");
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _db.KeyExistsAsync(key);
        }
        catch
        {
            Console.WriteLine($"Error checking key {key} in Redis");
            return false;
        }
    }
}