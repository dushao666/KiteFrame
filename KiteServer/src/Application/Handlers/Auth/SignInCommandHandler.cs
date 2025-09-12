namespace Application.Handlers.Auth;

/// <summary>
/// 登录命令处理器
/// </summary>
public class SignInCommandHandler : IRequestHandler<SignInCommand, LoginUserDto>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SignInCommandHandler> _logger;
    private readonly Infrastructure.Services.ICacheService _cacheService;

    public SignInCommandHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        IConfiguration configuration,
        ILogger<SignInCommandHandler> logger,
        Infrastructure.Services.ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<LoginUserDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.User? user = null;

            // 根据登录类型处理不同的登录方式
            switch (request.Type)
            {
                case LoginType.Password:
                    user = await HandlePasswordLogin(request);
                    break;
                case LoginType.SmsCode:
                    user = await HandleSmsCodeLogin(request);
                    break;
                case LoginType.DingTalk:
                    user = await HandleDingTalkLogin(request);
                    break;
                case LoginType.WeChat:
                    user = await HandleWeChatLogin(request);
                    break;
                default:
                    throw new BusinessException("不支持的登录类型");
            }

            if (user == null)
            {
                throw new BusinessException("登录失败，用户不存在或密码错误");
            }

            // 检查用户状态
            if (user.Status != 1)
            {
                throw new BusinessException("用户已被禁用，请联系管理员");
            }

            // 更新最后登录信息
            await UpdateLastLoginInfo(user, request.ClientIp);

            // 生成Token
            var loginResult = await GenerateTokens(user, request.RememberMe);

            _logger.LogInformation("用户 {UserName} 登录成功，IP: {ClientIp}", user.UserName, request.ClientIp);

            return loginResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户登录失败: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 处理账号密码登录
    /// </summary>
    private async Task<Domain.Entities.User?> HandlePasswordLogin(SignInCommand request)
    {
        using var context = _unitOfWork.CreateContext(false);

        // 对密码进行SHA512加密
        var hashedPassword = EncryptionHelper.Sha512(request.Password!);

        var user = await context.Users.GetFirstAsync(x =>
            x.UserName == request.UserName &&
            x.Password == hashedPassword);

        return user;
    }

    /// <summary>
    /// 处理短信验证码登录
    /// </summary>
    private async Task<Domain.Entities.User?> HandleSmsCodeLogin(SignInCommand request)
    {
        // 验证短信验证码
        var smsCodeCacheKey = CacheServiceExtensions.GenerateCacheKey("sms_code", request.Phone!);
        var cachedSmsCode = await _cacheService.GetAsync<string>(smsCodeCacheKey);

        if (string.IsNullOrEmpty(cachedSmsCode))
        {
            throw new BusinessException("验证码已过期，请重新获取");
        }

        if (cachedSmsCode != request.SmsCode)
        {
            throw new BusinessException("验证码错误");
        }

        // 验证成功后删除验证码缓存
        await _cacheService.RemoveAsync(smsCodeCacheKey);

        using var context = _unitOfWork.CreateContext(false);

        var user = await context.Users.GetFirstAsync(x => x.Phone == request.Phone);
        return user;
    }

    /// <summary>
    /// 处理钉钉登录
    /// </summary>
    private async Task<Domain.Entities.User?> HandleDingTalkLogin(SignInCommand request)
    {
        // TODO: 实现钉钉登录逻辑
        // 1. 通过钉钉授权码获取用户信息
        // 2. 根据钉钉用户ID查找本地用户

        using var context = _unitOfWork.CreateContext(false);

        // 暂时返回null，需要集成钉钉API
        throw new BusinessException("钉钉登录功能暂未实现");
    }

    /// <summary>
    /// 处理微信登录
    /// </summary>
    private async Task<Domain.Entities.User?> HandleWeChatLogin(SignInCommand request)
    {
        // TODO: 实现微信登录逻辑
        throw new BusinessException("微信登录功能暂未实现");
    }

    /// <summary>
    /// 更新最后登录信息
    /// </summary>
    private async Task UpdateLastLoginInfo(Domain.Entities.User user, string? clientIp)
    {
        using var context = _unitOfWork.CreateContext();

        user.LastLoginTime = DateTime.Now;
        user.LastLoginIp = clientIp;

        await context.Users.UpdateAsync(user);
    }

    /// <summary>
    /// 生成访问令牌和刷新令牌
    /// </summary>
    private async Task<LoginUserDto> GenerateTokens(Domain.Entities.User user, bool rememberMe)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "480");
        var refreshTokenExpiryDays = int.Parse(jwtSettings["RefreshTokenExpiryDays"] ?? "30");

        // 如果选择记住我，延长过期时间
        if (rememberMe)
        {
            expiryMinutes = refreshTokenExpiryDays * 24 * 60; // 转换为分钟
        }

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

        // 生成访问令牌
        var accessToken = JwtHelper.GenerateToken(
            claims,
            secretKey,
            "KiteServer",
            "KiteClient",
            expiresAt
        );

        // 生成刷新令牌
        var refreshToken = JwtHelper.GenerateRefreshToken();

        // 将刷新令牌存储到缓存中，用于后续刷新访问令牌
        var refreshTokenCacheKey = CacheServiceExtensions.GenerateTokenCacheKey(user.Id, "refresh_token");
        var refreshTokenExpiry = TimeSpan.FromDays(refreshTokenExpiryDays);
        await _cacheService.SetAsync(refreshTokenCacheKey, refreshToken, refreshTokenExpiry);

        // 存储用户会话信息到缓存
        var sessionCacheKey = CacheServiceExtensions.GenerateSessionCacheKey(user.Id, refreshToken);
        var sessionInfo = new
        {
            UserId = user.Id,
            UserName = user.UserName,
            LoginTime = DateTime.UtcNow,
            ClientIp = user.LastLoginIp,
            ExpiresAt = expiresAt
        };
        await _cacheService.SetAsync(sessionCacheKey, sessionInfo, refreshTokenExpiry);

        _logger.LogDebug("用户 {UserId} 的令牌已缓存，过期时间: {ExpiresAt}", user.Id, expiresAt);

        return new LoginUserDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            RealName = user.RealName,
            Email = user.Email,
            Phone = user.Phone,
            Avatar = user.Avatar,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            LastLoginTime = user.LastLoginTime,
            LastLoginIp = user.LastLoginIp
        };
    }
}
