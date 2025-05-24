﻿using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace MoongladePure.Caching;

public enum CacheDivision
{
    General,
    Post,
    Page,
    PostCountCategory,
    PostCountTag,
    PostCountFeatured,
    RssCategory
}

public class BlogMemoryCache(IMemoryCache memoryCache) : IBlogCache
{
    /* Create Key-Value mapping for cache divisions to workaround
     * https://github.com/aspnet/Caching/issues/422
     * This blog will need cache keys for post ids or category ids and need to be cleared later
     * Key               | Value
     * ------------------+--------------------------------------
     * PostCountCategory | { "<guid>", "<guid>", ... }
     * Post              | { "<guid>", "<guid>", "<guid"> ... }
     * General           | { "avatar", ... }
     */
    public ConcurrentDictionary<string, IList<string>> CacheDivision { get; } = new();

    public TItem GetOrCreate<TItem>(CacheDivision division, string key, Func<ICacheEntry, TItem> factory)
    {
        if (string.IsNullOrWhiteSpace(key)) return default;

        AddToDivision(division.ToString(), key);
        return memoryCache.GetOrCreate($"{division}-{key}", factory);
    }

    public Task<TItem> GetOrCreateAsync<TItem>(CacheDivision division, string key, Func<ICacheEntry, Task<TItem>> factory)
    {
        if (string.IsNullOrWhiteSpace(key)) return Task.FromResult(default(TItem));

        AddToDivision(division.ToString(), key);
        return memoryCache.GetOrCreateAsync($"{division}-{key}", factory);
    }

    public void RemoveAllCache()
    {
        var keys =
            from kvp in CacheDivision
            let prefix = kvp.Key
            from val in kvp.Value
            select $"{prefix}-{val}";

        foreach (var key in keys)
        {
            memoryCache.Remove(key);
        }
    }

    public void Remove(CacheDivision division)
    {
        if (!CacheDivision.ContainsKey(division.ToString())) return;

        var cacheKeys = CacheDivision[division.ToString()];
        if (cacheKeys.Any())
        {
            foreach (var key in cacheKeys)
            {
                memoryCache.Remove($"{division}-{key}");
            }
        }
    }

    public void Remove(CacheDivision division, string key)
    {
        if ((string.IsNullOrWhiteSpace(key)) || !CacheDivision.ContainsKey(division.ToString())) return;
        memoryCache.Remove($"{division}-{key}");
    }

    private void AddToDivision(string divisionKey, string cacheKey)
    {
        if (!CacheDivision.ContainsKey(divisionKey))
        {
            CacheDivision.TryAdd(divisionKey, new[] { cacheKey }.ToList());
        }

        if (!CacheDivision[divisionKey].Contains(cacheKey))
        {
            CacheDivision[divisionKey].Add(cacheKey);
        }
    }
}