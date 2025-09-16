using Shared.Events;

namespace Application.EventHandlers;

/// <summary>
/// 操作日志事件处理器
/// </summary>
public class OperationLogEventHandler : INotificationHandler<OperationLogEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<OperationLogEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OperationLogEventHandler(
        ISugarUnitOfWork<DBContext> unitOfWork,
        ILogger<OperationLogEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理操作日志事件
    /// </summary>
    public async Task Handle(OperationLogEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext(true);

            var operationLog = new OperationLog
            {
                UserId = notification.UserId,
                UserName = notification.UserName,
                Module = notification.Module,
                BusinessType = notification.BusinessType,
                Method = notification.Method,
                RequestMethod = notification.RequestMethod,
                OperatorType = notification.OperatorType,
                OperUrl = notification.OperUrl,
                OperIp = notification.OperIp,
                OperLocation = notification.OperLocation,
                OperParam = notification.OperParam,
                JsonResult = notification.JsonResult,
                Status = notification.Status,
                ErrorMsg = notification.ErrorMsg,
                OperTime = notification.OperTime,
                CostTime = notification.CostTime,
                CreateTime = DateTime.Now
            };

            await context.Db.Insertable(operationLog).ExecuteCommandAsync();
            context.Commit();

            _logger.LogDebug("记录操作日志成功，用户: {UserName}, 模块: {Module}, 操作: {BusinessType}", 
                notification.UserName, notification.Module, notification.BusinessType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理操作日志事件失败，用户: {UserName}, 模块: {Module}", 
                notification.UserName, notification.Module);
        }
    }
}
