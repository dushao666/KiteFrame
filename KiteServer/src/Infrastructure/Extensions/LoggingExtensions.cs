using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

/// <summary>
/// 日志扩展方法
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// 记录操作开始日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作名称</param>
    /// <param name="parameters">参数</param>
    public static void LogOperationStart(this ILogger logger, string operation, object? parameters = null)
    {
        if (parameters != null)
        {
            logger.LogInformation("开始执行操作: {Operation}, 参数: {@Parameters}", operation, parameters);
        }
        else
        {
            logger.LogInformation("开始执行操作: {Operation}", operation);
        }
    }

    /// <summary>
    /// 记录操作成功日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作名称</param>
    /// <param name="result">结果</param>
    /// <param name="elapsedMs">耗时（毫秒）</param>
    public static void LogOperationSuccess(this ILogger logger, string operation, object? result = null, long? elapsedMs = null)
    {
        var message = elapsedMs.HasValue 
            ? "操作执行成功: {Operation}, 耗时: {ElapsedMs}ms"
            : "操作执行成功: {Operation}";

        if (result != null)
        {
            if (elapsedMs.HasValue)
            {
                logger.LogInformation(message + ", 结果: {@Result}", operation, elapsedMs, result);
            }
            else
            {
                logger.LogInformation(message + ", 结果: {@Result}", operation, result);
            }
        }
        else
        {
            if (elapsedMs.HasValue)
            {
                logger.LogInformation(message, operation, elapsedMs);
            }
            else
            {
                logger.LogInformation(message, operation);
            }
        }
    }

    /// <summary>
    /// 记录操作失败日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作名称</param>
    /// <param name="exception">异常</param>
    /// <param name="parameters">参数</param>
    public static void LogOperationError(this ILogger logger, string operation, Exception exception, object? parameters = null)
    {
        if (parameters != null)
        {
            logger.LogError(exception, "操作执行失败: {Operation}, 参数: {@Parameters}", operation, parameters);
        }
        else
        {
            logger.LogError(exception, "操作执行失败: {Operation}", operation);
        }
    }

    /// <summary>
    /// 记录业务警告日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    public static void LogBusinessWarning(this ILogger logger, string message, params object[] args)
    {
        logger.LogWarning("[业务警告] " + message, args);
    }

    /// <summary>
    /// 记录安全相关日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    public static void LogSecurity(this ILogger logger, string message, params object[] args)
    {
        logger.LogWarning("[安全] " + message, args);
    }

    /// <summary>
    /// 记录性能相关日志
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="operation">操作</param>
    /// <param name="elapsedMs">耗时（毫秒）</param>
    /// <param name="threshold">阈值（毫秒）</param>
    public static void LogPerformance(this ILogger logger, string operation, long elapsedMs, long threshold = 1000)
    {
        if (elapsedMs > threshold)
        {
            logger.LogWarning("[性能] 操作 {Operation} 执行时间过长: {ElapsedMs}ms (阈值: {Threshold}ms)", 
                operation, elapsedMs, threshold);
        }
        else
        {
            logger.LogDebug("[性能] 操作 {Operation} 执行时间: {ElapsedMs}ms", operation, elapsedMs);
        }
    }
}
