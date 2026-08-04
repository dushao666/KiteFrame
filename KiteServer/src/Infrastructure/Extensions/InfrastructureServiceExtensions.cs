namespace Infrastructure.Extensions;

/// <summary>
/// 基础设施层服务扩展
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // HTTP 上下文访问器（当前用户服务等依赖）
        services.AddHttpContextAccessor();

        // 当前用户服务
        services.AddScoped<Services.ICurrentUser, Services.CurrentUserService>();

        return services;
    }
}
