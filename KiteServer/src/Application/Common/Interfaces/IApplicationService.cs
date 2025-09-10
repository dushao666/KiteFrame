namespace Application.Common.Interfaces;

/// <summary>
/// 应用服务基础接口
/// </summary>
public interface IApplicationService
{
}

/// <summary>
/// 当前用户接口
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// 用户ID
    /// </summary>
    long? UserId { get; }

    /// <summary>
    /// 用户名
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// 是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
}
