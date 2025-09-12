namespace Infrastructure.Exceptions;

/// <summary>
/// 用户友好异常
/// </summary>
[Serializable]
public class UserFriendlyException : Exception, IFriendlyException, IHasErrorCode, IHasLogLevel
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="logLevel">日志级别</param>
    public UserFriendlyException(
        string? message = null,
        Exception? innerException = null,
        LogLevel logLevel = LogLevel.Warning) : base(message, innerException)
    {
        LogLevel = logLevel;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="logLevel">日志级别</param>
    public UserFriendlyException(
        int code = 0,
        string? message = null,
        Exception? innerException = null,
        LogLevel logLevel = LogLevel.Warning) : base(message, innerException)
    {
        Code = code;
        LogLevel = logLevel;
    }
}

/// <summary>
/// 参数错误异常
/// </summary>
public class MissingException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public MissingException()
        : base(0, "请求参数有误", null, LogLevel.Information)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public MissingException(string message)
        : base(0, message, null, LogLevel.Information)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">异常消息</param>
    public MissingException(int code, string message)
        : base(code, message, null, LogLevel.Information)
    {
    }
}

/// <summary>
/// 业务异常
/// </summary>
public class BusinessException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public BusinessException(string message)
        : base(0, message, null, LogLevel.Warning)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">异常消息</param>
    public BusinessException(int code, string message)
        : base(code, message, null, LogLevel.Warning)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public BusinessException(int code, string message, Exception innerException)
        : base(code, message, innerException, LogLevel.Warning)
    {
    }
}

/// <summary>
/// 验证异常
/// </summary>
public class ValidationException : UserFriendlyException
{
    /// <summary>
    /// 验证错误详情
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public ValidationException(string message)
        : base(0, message, null, LogLevel.Information)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="errors">验证错误详情</param>
    public ValidationException(Dictionary<string, string[]> errors)
        : base(0, "验证失败", null, LogLevel.Information)
    {
        Errors = errors;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="errors">验证错误详情</param>
    public ValidationException(string message, Dictionary<string, string[]> errors)
        : base(0, message, null, LogLevel.Information)
    {
        Errors = errors;
    }
}

/// <summary>
/// 未授权异常
/// </summary>
public class UnauthorizedException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public UnauthorizedException(string message = "未授权访问")
        : base(401, message, null, LogLevel.Warning)
    {
    }
}

/// <summary>
/// 禁止访问异常
/// </summary>
public class ForbiddenException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public ForbiddenException(string message = "禁止访问")
        : base(403, message, null, LogLevel.Warning)
    {
    }
}

/// <summary>
/// 资源未找到异常
/// </summary>
public class NotFoundException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public NotFoundException(string message = "资源未找到")
        : base(404, message, null, LogLevel.Information)
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="key">资源键</param>
    public NotFoundException(string name, object key)
        : base(404, $"资源 '{name}' ({key}) 未找到", null, LogLevel.Information)
    {
    }
}

/// <summary>
/// 冲突异常
/// </summary>
public class ConflictException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public ConflictException(string message = "资源冲突")
        : base(409, message, null, LogLevel.Warning)
    {
    }
}

/// <summary>
/// 服务器内部错误异常
/// </summary>
public class InternalServerErrorException : UserFriendlyException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public InternalServerErrorException(string message = "服务器内部错误", Exception? innerException = null)
        : base(500, message, innerException, LogLevel.Error)
    {
    }
}
