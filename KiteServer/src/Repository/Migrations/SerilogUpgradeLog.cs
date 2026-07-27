using DbUp.Engine.Output;

namespace Repository.Migrations;

/// <summary>
/// 将 DbUp 引擎日志转发到 Serilog 的适配器
/// </summary>
public sealed class SerilogUpgradeLog : IUpgradeLog
{
    /// <summary>
    /// 记录跟踪级别日志
    /// </summary>
    /// <param name="format">日志格式（DbUp 使用 {0} 位置占位符，Serilog 原生支持）</param>
    /// <param name="args">占位符参数</param>
    public void LogTrace(string format, params object[] args) => Log.Verbose(format, args);

    /// <summary>
    /// 记录调试级别日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">占位符参数</param>
    public void LogDebug(string format, params object[] args) => Log.Debug(format, args);

    /// <summary>
    /// 记录信息级别日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">占位符参数</param>
    public void LogInformation(string format, params object[] args) => Log.Information(format, args);

    /// <summary>
    /// 记录警告级别日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">占位符参数</param>
    public void LogWarning(string format, params object[] args) => Log.Warning(format, args);

    /// <summary>
    /// 记录错误级别日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">占位符参数</param>
    public void LogError(string format, params object[] args) => Log.Error(format, args);

    /// <summary>
    /// 记录错误级别日志（携带异常）
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="format">日志格式</param>
    /// <param name="args">占位符参数</param>
    public void LogError(Exception ex, string format, params object[] args) => Log.Error(ex, format, args);
}
