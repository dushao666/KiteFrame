namespace Application.Handlers.Monitor;

/// <summary>
/// 操作日志事件处理器：将操作日志落库
/// 监控逻辑不影响主业务流程，异常在此捕获并记录日志
/// </summary>
public class OperationLogEventHandler : INotificationHandler<OperationLogEvent>
{
    private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
    private readonly ILogger<OperationLogEventHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="unitOfWork">工作单元</param>
    /// <param name="logger">日志</param>
    public OperationLogEventHandler(ISugarUnitOfWork<DBContext> unitOfWork, ILogger<OperationLogEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 处理操作日志事件
    /// </summary>
    /// <param name="notification">操作日志事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task Handle(OperationLogEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _unitOfWork.CreateContext();

            // 事件与实体字段同名，使用 Mapster 自动映射
            var operationLog = notification.Adapt<OperationLog>();
            await context.OperationLogs.InsertAsync(operationLog);
            context.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理操作日志事件失败，模块: {Module}，方法: {Method}", notification.Module, notification.Method);
        }
    }
}
