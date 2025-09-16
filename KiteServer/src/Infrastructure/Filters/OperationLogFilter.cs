using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Attributes;
using Shared.Events;
using System.Diagnostics;
using System.Text.Json;
using System.Security.Claims;
using MediatR;

namespace Infrastructure.Filters;

/// <summary>
/// 操作日志过滤器
/// </summary>
public class OperationLogFilter : IAsyncActionFilter
{
    private readonly IMediator _mediator;
    private readonly ILogger<OperationLogFilter> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OperationLogFilter(IMediator mediator, ILogger<OperationLogFilter> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 执行操作日志记录
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.Now;
        
        // 检查是否需要记录操作日志
        var operationLogAttr = GetOperationLogAttribute(context);
        if (operationLogAttr == null)
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;
        var user = context.HttpContext.User;
        
        // 获取用户信息
        var userId = GetUserId(user);
        var userName = GetUserName(user);
        
        // 获取请求参数
        var operParam = string.Empty;
        if (operationLogAttr.LogParams)
        {
            operParam = await GetRequestParamsAsync(context);
        }

        ActionExecutedContext? executedContext = null;
        Exception? exception = null;
        
        try
        {
            executedContext = await next();
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // 发布操作日志事件
            await PublishOperationLogEventAsync(
                operationLogAttr,
                request,
                userId,
                userName,
                operParam,
                executedContext,
                exception,
                startTime,
                stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// 获取操作日志特性
    /// </summary>
    private OperationLogAttribute? GetOperationLogAttribute(ActionExecutingContext context)
    {
        // 先检查方法级别的特性
        var methodAttr = context.ActionDescriptor.EndpointMetadata
            .OfType<OperationLogAttribute>()
            .FirstOrDefault();
        
        if (methodAttr != null)
            return methodAttr;

        // 再检查控制器级别的特性
        var controllerAttr = context.Controller.GetType()
            .GetCustomAttributes(typeof(OperationLogAttribute), true)
            .OfType<OperationLogAttribute>()
            .FirstOrDefault();

        return controllerAttr;
    }

    /// <summary>
    /// 获取用户ID
    /// </summary>
    private long? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    /// <summary>
    /// 获取用户名
    /// </summary>
    private string? GetUserName(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// 获取请求参数
    /// </summary>
    private async Task<string> GetRequestParamsAsync(ActionExecutingContext context)
    {
        try
        {
            var request = context.HttpContext.Request;
            var parameters = new Dictionary<string, object?>();

            // 获取路由参数
            foreach (var param in context.ActionArguments)
            {
                parameters[param.Key] = param.Value;
            }

            // 获取查询参数
            foreach (var query in request.Query)
            {
                parameters[$"query_{query.Key}"] = query.Value.ToString();
            }

            return JsonSerializer.Serialize(parameters, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "获取请求参数失败");
            return string.Empty;
        }
    }

    /// <summary>
    /// 获取返回结果
    /// </summary>
    private string GetJsonResult(ActionExecutedContext? executedContext, OperationLogAttribute operationLogAttr)
    {
        if (!operationLogAttr.LogResult || executedContext?.Result == null)
            return string.Empty;

        try
        {
            return JsonSerializer.Serialize(executedContext.Result, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "序列化返回结果失败");
            return string.Empty;
        }
    }

    /// <summary>
    /// 发布操作日志事件
    /// </summary>
    private async Task PublishOperationLogEventAsync(
        OperationLogAttribute operationLogAttr,
        HttpRequest request,
        long? userId,
        string? userName,
        string operParam,
        ActionExecutedContext? executedContext,
        Exception? exception,
        DateTime startTime,
        long costTime)
    {
        try
        {
            var operationLogEvent = new OperationLogEvent
            {
                UserId = userId,
                UserName = userName,
                Module = operationLogAttr.Module,
                BusinessType = operationLogAttr.BusinessType,
                Method = $"{request.Method} {request.Path}",
                RequestMethod = request.Method,
                OperatorType = userId.HasValue ? 1 : 2, // 1：管理员，2：用户
                OperUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                OperIp = GetClientIpAddress(request),
                OperLocation = "未知", // TODO: 实现IP归属地查询
                OperParam = operParam,
                JsonResult = GetJsonResult(executedContext, operationLogAttr),
                Status = exception == null ? 1 : 0,
                ErrorMsg = exception?.Message,
                OperTime = startTime,
                CostTime = costTime
            };

            await _mediator.Publish(operationLogEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布操作日志事件失败");
        }
    }

    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    private string? GetClientIpAddress(HttpRequest request)
    {
        // 尝试从各种可能的头部获取真实IP
        var ipAddress = request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            // X-Forwarded-For 可能包含多个IP，取第一个
            return ipAddress.Split(',')[0].Trim();
        }

        ipAddress = request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }

        ipAddress = request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }

        return request.HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
