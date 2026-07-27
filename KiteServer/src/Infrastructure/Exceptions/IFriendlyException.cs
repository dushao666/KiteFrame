namespace Infrastructure.Exceptions;

/// <summary>
/// 错误码接口
/// </summary>
public interface IHasErrorCode
{
    /// <summary>
    /// 错误码
    /// </summary>
    int Code { get; }
}

/// <summary>
/// 日志级别接口
/// </summary>
public interface IHasLogLevel
{
    /// <summary>
    /// 日志级别
    /// </summary>
    LogLevel LogLevel { get; set; }
}

/// <summary>
/// 友好异常接口
/// </summary>
public interface IFriendlyException
{
    /// <summary>
    /// 错误码
    /// </summary>
    int Code { get; }
    
    /// <summary>
    /// 日志级别
    /// </summary>
    LogLevel LogLevel { get; set; }
}
