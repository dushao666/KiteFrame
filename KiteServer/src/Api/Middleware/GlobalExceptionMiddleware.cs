namespace Api.Middleware;

/// <summary>
/// 全局异常处理中间件：将未捕获的异常统一转换为 ApiResult 响应
/// 友好异常（<see cref="IFriendlyException"/>）按其错误码映射 HTTP 状态码并返回异常消息；
/// 其余异常仅记录日志并返回通用错误消息，避免向客户端泄露堆栈等敏感信息。
/// 采用自定义中间件而非框架 IExceptionHandler 机制，避免其对框架版本的解析行为依赖，跨版本更稳健
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next">管道委托</param>
    /// <param name="logger">日志</param>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 执行请求处理，捕获异常并写入统一格式的响应
    /// </summary>
    /// <param name="httpContext">HTTP 上下文</param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    /// <summary>
    /// 处理异常并写入统一格式的响应
    /// </summary>
    /// <param name="httpContext">HTTP 上下文</param>
    /// <param name="exception">捕获到的异常</param>
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        // 响应已经开始写入时无法再改写响应体，仅记录日志
        if (httpContext.Response.HasStarted)
        {
            _logger.LogWarning(exception, "响应已开始写入，无法统一处理异常: {Path}", httpContext.Request.Path);
            return;
        }

        int businessCode;
        string message;

        if (exception is IFriendlyException friendly)
        {
            // 友好异常：按异常自带的日志级别记录，消息可直接返回给客户端
            _logger.Log(friendly.LogLevel, exception, "请求处理失败: {Path} - {Message}", httpContext.Request.Path, exception.Message);

            businessCode = friendly.Code == 0 ? StatusCodes.Status400BadRequest : friendly.Code;
            message = exception.Message;
        }
        else
        {
            // 未知异常：记录完整堆栈，但不向客户端暴露细节
            _logger.LogError(exception, "请求处理发生未预期的异常: {Path}", httpContext.Request.Path);

            businessCode = StatusCodes.Status500InternalServerError;
            message = "服务器内部错误，请稍后重试";
        }

        // HTTP 状态码：友好异常携带的标准状态码（401/403/404/409/500 等）直接采用，其余按 400 处理
        httpContext.Response.Clear();
        httpContext.Response.StatusCode = businessCode >= 400 ? businessCode : StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        var result = ApiResult.Fail(message, businessCode);
        await httpContext.Response.WriteAsJsonAsync(result);
    }
}
