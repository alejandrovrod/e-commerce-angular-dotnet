using ECommerce.Product.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Product.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }

            var result = JsonSerializer.Deserialize<T>(value, _jsonOptions);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
            return default(T);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                // Default expiration: 15 minutes
                options.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
            }

            await _cache.SetStringAsync(key, jsonValue, options);
            _logger.LogDebug("Value cached for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Cache entry removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache entry for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            // Note: Redis doesn't support pattern-based deletion directly
            // This is a simplified implementation. In production, you might want to:
            // 1. Use Redis SCAN command to find matching keys
            // 2. Use a more sophisticated pattern matching library
            // 3. Implement key naming conventions that make pattern deletion easier

            _logger.LogWarning("Pattern-based cache removal not fully implemented for pattern: {Pattern}. Consider using specific keys.", pattern);
            
            // For now, we'll just log this as a limitation
            // In a real implementation, you'd implement the SCAN logic here
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if cache key exists: {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = factory();
        await SetAsync(key, value, expiration);
        return value;
    }

    public async Task<T> GetOrSetAsync<T>(string key, T defaultValue, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        await SetAsync(key, defaultValue, expiration);
        return defaultValue;
    }

    public async Task IncrementAsync(string key, long value = 1, TimeSpan? expiration = null)
    {
        try
        {
            var currentValue = await GetAsync<long?>(key) ?? 0L;
            var newValue = currentValue + value;
            await SetAsync(key, newValue, expiration);
            _logger.LogDebug("Incremented cache value for key: {Key} by {Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache value for key: {Key}", key);
        }
    }

    public async Task DecrementAsync(string key, long value = 1, TimeSpan? expiration = null)
    {
        try
        {
            var currentValue = await GetAsync<long?>(key) ?? 0L;
            var newValue = Math.Max(0, currentValue - value);
            await SetAsync(key, newValue, expiration);
            _logger.LogDebug("Decremented cache value for key: {Key} by {Value}", key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decrementing cache value for key: {Key}", key);
        }
    }
}
