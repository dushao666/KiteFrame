namespace Application.Handlers.Auth;

/// <summary>
/// 刷新Token命令处理器
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly Infrastructure.Services.ICacheService _cacheService;

    public RefreshTokenCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        IConfiguration configuration,
        ILogger<RefreshTokenCommandHandler> logger,
        Infrastructure.Services.ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<RefreshTokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new BusinessException("刷新令牌不能为空");
            }

            // 验证刷新令牌
            var userId = await ValidateRefreshToken(request.RefreshToken);
            if (userId == 0)
            {
                throw new BusinessException("无效的刷新令牌");
            }

            // 获取用户信息
            using var context = _unitOfWork.CreateContext(false);
            var user = await context.Users.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            // 检查用户状态
            if (user.Status != 1)
            {
                throw new BusinessException("用户已被禁用");
            }

            // 生成新的令牌
            var result = await GenerateNewTokens(user, request.RefreshToken);

            _logger.LogInformation("用户 {UserName} 刷新Token成功", user.UserName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新Token失败: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 验证刷新令牌并返回用户ID
    /// </summary>
    private async Task<long> ValidateRefreshToken(string refreshToken)
    {
        try
        {
            // 遍历所有可能的用户ID来查找匹配的刷新令牌
            // 注意：这是一个简化的实现，生产环境建议使用更高效的方式
            // 例如：在刷新令牌中包含用户ID信息，或使用专门的刷新令牌存储表

            using var context = _unitOfWork.CreateContext(false);
            var users = await context.Users.GetListAsync(x => x.Status == 1);

            foreach (var user in users)
            {
                var refreshTokenCacheKey = CacheServiceExtensions.GenerateTokenCacheKey(user.Id, "refresh_token");
                var cachedRefreshToken = await _cacheService.GetAsync<string>(refreshTokenCacheKey);

                if (cachedRefreshToken == refreshToken)
                {
                    return user.Id;
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证刷新令牌失败");
            return 0;
        }
    }

    /// <summary>
    /// 生成新的访问令牌和刷新令牌
    /// </summary>
    private async Task<RefreshTokenDto> GenerateNewTokens(Domain.Entities.User user, string oldRefreshToken)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "480");
        var refreshTokenExpiryDays = int.Parse(jwtSettings["RefreshTokenExpiryDays"] ?? "30");

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // 创建声明
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.MobilePhone, user.Phone ?? string.Empty),
            new("RealName", user.RealName ?? string.Empty),
            new("Avatar", user.Avatar ?? string.Empty),
            new(ClaimTypes.Expiration, expiresAt.ToString("yyyy-MM-dd HH:mm:ss"))
        };

        // 生成新的访问令牌
        var accessToken = JwtHelper.GenerateToken(
            claims,
            secretKey,
            "KiteServer",
            "KiteClient",
            expiresAt
        );

        // 生成新的刷新令牌
        var newRefreshToken = JwtHelper.GenerateRefreshToken();

        // 删除旧的刷新令牌缓存
        var oldRefreshTokenCacheKey = CacheServiceExtensions.GenerateTokenCacheKey(user.Id, "refresh_token");
        await _cacheService.RemoveAsync(oldRefreshTokenCacheKey);

        // 删除旧的会话缓存
        var oldSessionCacheKey = CacheServiceExtensions.GenerateSessionCacheKey(user.Id, oldRefreshToken);
        await _cacheService.RemoveAsync(oldSessionCacheKey);

        // 存储新的刷新令牌到缓存中
        var newRefreshTokenCacheKey = CacheServiceExtensions.GenerateTokenCacheKey(user.Id, "refresh_token");
        var refreshTokenExpiry = TimeSpan.FromDays(refreshTokenExpiryDays);
        await _cacheService.SetAsync(newRefreshTokenCacheKey, newRefreshToken, refreshTokenExpiry);

        // 存储新的用户会话信息到缓存
        var newSessionCacheKey = CacheServiceExtensions.GenerateSessionCacheKey(user.Id, newRefreshToken);
        var sessionInfo = new
        {
            UserId = user.Id,
            UserName = user.UserName,
            LoginTime = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
        await _cacheService.SetAsync(newSessionCacheKey, sessionInfo, refreshTokenExpiry);

        _logger.LogDebug("用户 {UserId} 的令牌已刷新，过期时间: {ExpiresAt}", user.Id, expiresAt);

        return new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt
        };
    }
}
