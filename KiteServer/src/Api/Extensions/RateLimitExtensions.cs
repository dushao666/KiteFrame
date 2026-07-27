namespace Api.Extensions;

/// <summary>
/// 限流扩展方法
/// </summary>
public static class RateLimitExtensions
{
    /// <summary>
    /// 添加限流服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddRateLimitServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitSettings = configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>();
        
        if (rateLimitSettings?.Enabled != true)
        {
            return services;
        }

        services.AddRateLimiter(options =>
        {
            // 设置拒绝响应
            options.RejectionStatusCode = rateLimitSettings.StatusCode;
            
            // 当请求被拒绝时的处理
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = rateLimitSettings.StatusCode;
                
                // 如果存在重试时间，添加到响应头
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    code = rateLimitSettings.StatusCode,
                    message = rateLimitSettings.Message,
                    timestamp = DateTime.Now
                }, cancellationToken: cancellationToken);
            };

            // 根据配置选择限流策略
            switch (rateLimitSettings.PolicyType.ToLower())
            {
                case "fixed":
                    AddFixedWindowPolicy(options, rateLimitSettings);
                    break;
                case "sliding":
                    AddSlidingWindowPolicy(options, rateLimitSettings);
                    break;
                case "token":
                    AddTokenBucketPolicy(options, rateLimitSettings);
                    break;
                case "concurrency":
                    AddConcurrencyPolicy(options, rateLimitSettings);
                    break;
                default:
                    AddFixedWindowPolicy(options, rateLimitSettings);
                    break;
            }
        });

        return services;
    }

    /// <summary>
    /// 添加固定窗口限流策略
    /// </summary>
    private static void AddFixedWindowPolicy(RateLimiterOptions options, RateLimitSettings settings)
    {
        var config = settings.FixedWindow ?? new FixedWindowConfig();
        
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = GetPartitionKey(httpContext, settings);
            
            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = config.PermitLimit,
                Window = TimeSpan.FromSeconds(config.Window),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = config.QueueLimit
            });
        });
    }

    /// <summary>
    /// 添加滑动窗口限流策略
    /// </summary>
    private static void AddSlidingWindowPolicy(RateLimiterOptions options, RateLimitSettings settings)
    {
        var config = settings.SlidingWindow ?? new SlidingWindowConfig();
        
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = GetPartitionKey(httpContext, settings);
            
            return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = config.PermitLimit,
                Window = TimeSpan.FromSeconds(config.Window),
                SegmentsPerWindow = config.SegmentsPerWindow,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = config.QueueLimit
            });
        });
    }

    /// <summary>
    /// 添加令牌桶限流策略
    /// </summary>
    private static void AddTokenBucketPolicy(RateLimiterOptions options, RateLimitSettings settings)
    {
        var config = settings.TokenBucket ?? new TokenBucketConfig();
        
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = GetPartitionKey(httpContext, settings);
            
            return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = config.TokenLimit,
                TokensPerPeriod = config.TokensPerPeriod,
                ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriod),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = config.QueueLimit
            });
        });
    }

    /// <summary>
    /// 添加并发限流策略
    /// </summary>
    private static void AddConcurrencyPolicy(RateLimiterOptions options, RateLimitSettings settings)
    {
        var config = settings.Concurrency ?? new ConcurrencyConfig();
        
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = GetPartitionKey(httpContext, settings);
            
            return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = config.PermitLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = config.QueueLimit
            });
        });
    }

    /// <summary>
    /// 获取分区键（用于区分不同的限流对象）
    /// </summary>
    private static string GetPartitionKey(HttpContext httpContext, RateLimitSettings settings)
    {
        var keys = new List<string>();

        // 如果启用IP限流，添加IP地址
        if (settings.EnableIpRateLimit)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            keys.Add($"ip:{ipAddress}");
        }

        // 如果启用用户限流，添加用户标识
        if (settings.EnableUserRateLimit && httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.Identity.Name ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                keys.Add($"user:{userId}");
            }
        }

        // 如果没有任何键，使用全局键
        return keys.Count > 0 ? string.Join("_", keys) : "global";
    }
}
