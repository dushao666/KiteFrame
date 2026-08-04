namespace Application.Handlers.Monitor;

/// <summary>
/// 用户登录事件处理器：记录登录日志并维护在线用户
/// 监控逻辑不影响主业务流程，异常在此捕获并记录日志
/// </summary>
public class UserLoginEventHandler : INotificationHandler<UserLoginEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserLoginEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="logger">日志</param>
    public UserLoginEventHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UserLoginEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理用户登录事件
    /// </summary>
    /// <param name="notification">登录事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext();

            // 记录登录日志（成功与失败均记录）
            var loginLog = new LoginLog
            {
                UserId = notification.UserId == 0 ? null : notification.UserId,
                UserName = notification.UserName,
                IpAddress = notification.IpAddress,
                IpLocation = notification.IpLocation,
                Browser = notification.Browser,
                Os = notification.Os,
                Status = notification.Status,
                Message = notification.Message,
                LoginTime = notification.LoginTime
            };
            await context.LoginLogs.InsertAsync(loginLog);

            // 仅登录成功时维护在线用户记录
            if (notification.Status == 1 && !string.IsNullOrEmpty(notification.SessionId))
            {
                var existing = await context.OnlineUsers.GetFirstAsync(x => x.SessionId == notification.SessionId);
                if (existing == null)
                {
                    var onlineUser = new OnlineUser
                    {
                        SessionId = notification.SessionId,
                        UserId = notification.UserId,
                        UserName = notification.UserName,
                        RealName = notification.RealName,
                        DeptId = notification.DeptId,
                        DeptName = notification.DeptName,
                        IpAddress = notification.IpAddress,
                        IpLocation = notification.IpLocation,
                        Browser = notification.Browser,
                        Os = notification.Os,
                        LoginTime = notification.LoginTime,
                        LastAccessTime = notification.LoginTime,
                        ExpireTime = notification.ExpireTime,
                        Status = 1
                    };
                    await context.OnlineUsers.InsertAsync(onlineUser);
                }
                else
                {
                    // 幂等处理：同一会话重复登录（如刷新令牌）时更新现有记录
                    existing.IpAddress = notification.IpAddress;
                    existing.IpLocation = notification.IpLocation;
                    existing.Browser = notification.Browser;
                    existing.Os = notification.Os;
                    existing.LastAccessTime = notification.LoginTime;
                    existing.ExpireTime = notification.ExpireTime;
                    existing.Status = 1;
                    existing.UpdateTime = DateTime.Now;
                    await context.OnlineUsers.UpdateAsync(existing);
                }
            }

            context.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理用户登录事件失败，用户: {UserName}", notification.UserName);
        }
    }
}
