namespace Shared.Models;

/// <summary>
/// API 统一返回结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// 成功结果
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="message">消息</param>
    /// <returns></returns>
    public static ApiResult<T> Ok(T? data = default, string message = "操作成功")
    {
        return new ApiResult<T>
        {
            Success = true,
            Code = 200,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// 失败结果
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    public static ApiResult<T> Fail(string message, int code = 400)
    {
        return new ApiResult<T>
        {
            Success = false,
            Code = code,
            Message = message
        };
    }
}

/// <summary>
/// API 统一返回结果（无数据）
/// </summary>
public class ApiResult : ApiResult<object>
{
    /// <summary>
    /// 成功结果
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns></returns>
    public static ApiResult Ok(string message = "操作成功")
    {
        return new ApiResult
        {
            Success = true,
            Code = 200,
            Message = message
        };
    }

    /// <summary>
    /// 失败结果
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    public static new ApiResult Fail(string message, int code = 400)
    {
        return new ApiResult
        {
            Success = false,
            Code = code,
            Message = message
        };
    }
}
