using Shared.Events;

namespace Application.Handlers.Auth;

/// <summary>
/// 退出登录命令处理器
/// </summary>
public class SignOutCommandHandler : IRequestHandler<SignOutCommand, bool>
{
    private readonly Infrastructure.Services.ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly ILogger<SignOutCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SignOutCommandHandler(
        Infrastructure.Services.ICacheService cacheService,
        IMediator mediator,
        ILogger<SignOutCommandHandler> logger)
    {
        _cacheService = cacheService;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 处理退出登录命令
    /// </summary>
    public async Task<bool> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 清除缓存中的令牌信息
            var refreshTokenCacheKey = CacheServiceExtensions.GenerateTokenCacheKey(request.UserId, "refresh_token");
            await _cacheService.RemoveAsync(refreshTokenCacheKey);

            var sessionCacheKey = CacheServiceExtensions.GenerateSessionCacheKey(request.UserId, request.SessionId);
            await _cacheService.RemoveAsync(sessionCacheKey);

            // 发布用户退出登录事件
            var logoutEvent = new UserLogoutEvent
            {
                UserId = request.UserId,
                UserName = request.UserName,
                SessionId = request.SessionId,
                IpAddress = request.ClientIp,
                LogoutType = request.LogoutType,
                LogoutTime = DateTime.Now
            };

            await _mediator.Publish(logoutEvent, cancellationToken);

            _logger.LogInformation("用户 {UserName} 退出登录成功，IP: {ClientIp}", request.UserName, request.ClientIp);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用户 {UserName} 退出登录失败", request.UserName);
            return false;
        }
    }
}
