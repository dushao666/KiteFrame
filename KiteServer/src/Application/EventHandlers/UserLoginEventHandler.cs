using Shared.Events;

namespace Application.EventHandlers;

/// <summary>
/// 用户登录事件处理器
/// </summary>
public class UserLoginEventHandler : INotificationHandler<UserLoginEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<UserLoginEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UserLoginEventHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        ILogger<UserLoginEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理用户登录事件
    /// </summary>
    public async Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            // 1. 记录登录日志
            await RecordLoginLogAsync(context, notification);

            // 2. 如果登录成功，记录在线用户
            if (notification.Status == 1)
            {
                await RecordOnlineUserAsync(context, notification);
            }

            context.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理用户登录事件失败，用户: {UserName}", notification.UserName);
        }
    }

    /// <summary>
    /// 记录登录日志
    /// </summary>
    private async Task RecordLoginLogAsync(object context, UserLoginEvent notification)
    {
        var loginLog = new LoginLog
        {
            UserId = notification.UserId,
            UserName = notification.UserName,
            IpAddress = notification.IpAddress,
            IpLocation = notification.IpLocation,
            Browser = notification.Browser,
            Os = notification.Os,
            Status = notification.Status,
            Message = notification.Message ?? (notification.Status == 1 ? "登录成功" : "登录失败"),
            LoginTime = notification.LoginTime,
            CreateTime = DateTime.Now
        };

        await ((dynamic)context).Db.Insertable(loginLog).ExecuteCommandAsync();
        _logger.LogInformation("记录登录日志成功，用户: {UserName}, 状态: {Status}", notification.UserName, notification.Status);
    }

    /// <summary>
    /// 记录在线用户
    /// </summary>
    private async Task RecordOnlineUserAsync(object context, UserLoginEvent notification)
    {
        // 先将该用户的其他会话设为离线
        var db = ((dynamic)context).Db;
        await db.Updateable<OnlineUser>()
            .SetColumns("Status", 0)
            .SetColumns("UpdateTime", DateTime.Now)
            .Where("UserId = @UserId", new { UserId = notification.UserId })
            .ExecuteCommandAsync();

        // 创建新的在线用户记录
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
            Status = 1,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        };

        await ((dynamic)context).Db.Insertable(onlineUser).ExecuteCommandAsync();
        _logger.LogInformation("记录在线用户成功，用户: {UserName}, 会话: {SessionId}", notification.UserName, notification.SessionId);
    }
}
