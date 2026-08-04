namespace Application.Handlers.Monitor;

/// <summary>
/// 用户退出登录事件处理器：将对应会话的在线用户标记为离线
/// 监控逻辑不影响主业务流程，异常在此捕获并记录日志
/// </summary>
public class UserLogoutEventHandler : INotificationHandler<UserLogoutEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserLogoutEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="logger">日志</param>
    public UserLogoutEventHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<UserLogoutEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理用户退出登录事件
    /// </summary>
    /// <param name="notification">退出事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task Handle(UserLogoutEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(notification.SessionId))
            {
                return;
            }

            using var context = _unitOfWork.CreateContext();

            var onlineUser = await context.OnlineUsers.GetFirstAsync(x =>
                x.SessionId == notification.SessionId && x.Status == 1);

            if (onlineUser != null)
            {
                onlineUser.Status = 0;
                onlineUser.LastAccessTime = notification.LogoutTime;
                onlineUser.UpdateTime = DateTime.Now;
                await context.OnlineUsers.UpdateAsync(onlineUser);
                context.Commit();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理用户退出事件失败，用户: {UserName}", notification.UserName);
        }
    }
}
