namespace Application.Handlers.Auth;

/// <summary>
/// 发送短信验证码命令处理器
/// </summary>
public class SendSmsCodeCommandHandler : IRequestHandler<SendSmsCodeCommand, bool>
{
    private readonly Infrastructure.Services.ICacheService _cacheService;
    private readonly ILogger<SendSmsCodeCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cacheService">缓存服务</param>
    /// <param name="logger">日志记录器</param>
    public SendSmsCodeCommandHandler(
        Infrastructure.Services.ICacheService cacheService,
        ILogger<SendSmsCodeCommandHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// 处理发送短信验证码命令
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发送结果</returns>
    public async Task<bool> Handle(SendSmsCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 检查发送频率限制
            var rateLimitKey = CacheServiceExtensions.GenerateCacheKey("sms_rate_limit", request.Phone);
            var lastSendTime = await _cacheService.GetAsync<DateTime?>(rateLimitKey, cancellationToken);
            
            if (lastSendTime.HasValue && DateTime.UtcNow.Subtract(lastSendTime.Value).TotalSeconds < 60)
            {
                throw new BusinessException("发送过于频繁，请稍后再试");
            }

            // 生成6位数字验证码
            var smsCode = GenerateSmsCode();
            
            // 将验证码存储到缓存中，有效期5分钟
            var smsCodeCacheKey = CacheServiceExtensions.GenerateCacheKey("sms_code", request.Phone);
            await _cacheService.SetAsync(smsCodeCacheKey, smsCode, TimeSpan.FromMinutes(5), cancellationToken);

            // 记录发送时间，用于频率限制
            await _cacheService.SetAsync(rateLimitKey, DateTime.UtcNow, TimeSpan.FromMinutes(1), cancellationToken);

            // TODO: 这里应该调用实际的短信服务发送验证码
            // 目前只是模拟发送，实际项目中需要集成阿里云、腾讯云等短信服务
            var success = await SendSmsAsync(request.Phone, smsCode, request.CodeType);

            if (success)
            {
                _logger.LogInformation("短信验证码发送成功，手机号: {Phone}, IP: {ClientIp}", 
                    request.Phone, request.ClientIp);
            }
            else
            {
                // 发送失败时清除缓存
                await _cacheService.RemoveAsync(smsCodeCacheKey, cancellationToken);
                _logger.LogWarning("短信验证码发送失败，手机号: {Phone}, IP: {ClientIp}", 
                    request.Phone, request.ClientIp);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送短信验证码失败，手机号: {Phone}", request.Phone);
            throw;
        }
    }

    /// <summary>
    /// 生成短信验证码
    /// </summary>
    /// <returns>6位数字验证码</returns>
    private static string GenerateSmsCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    /// <summary>
    /// 发送短信验证码
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="code">验证码</param>
    /// <param name="codeType">验证码类型</param>
    /// <returns>发送结果</returns>
    private async Task<bool> SendSmsAsync(string phone, string code, string codeType)
    {
        try
        {
            // TODO: 集成实际的短信服务
            // 这里只是模拟发送过程
            await Task.Delay(100); // 模拟网络延迟
            
            _logger.LogDebug("模拟发送短信验证码: {Phone} -> {Code} (类型: {CodeType})", 
                phone, code, codeType);
            
            // 在开发环境下，可以直接返回true并在日志中输出验证码
            // 生产环境中应该调用真实的短信服务
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "短信发送服务异常，手机号: {Phone}", phone);
            return false;
        }
    }
}
