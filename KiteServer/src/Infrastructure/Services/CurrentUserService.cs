namespace Infrastructure.Services;

/// <summary>
/// 当前用户服务实现
/// </summary>
public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst("userName")?.Value;

    /// <summary>
    /// 是否已认证
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
