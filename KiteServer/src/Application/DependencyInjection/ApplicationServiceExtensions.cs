using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Behaviors;
using FluentValidation;

namespace Application.DependencyInjection;

/// <summary>
/// 应用层服务扩展
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 添加查询服务
        services.AddQueryServices();

        // 注册 MediatR：命令处理器与事件处理器由 MediatR 自动发现，
        // 并挂载校验管道行为（校验失败抛出 ValidationException，由全局异常处理器统一转换）
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // 注册 FluentValidation 校验器（按程序集扫描）
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

        return services;
    }
    
    /// <summary>
    /// 添加查询服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        // 自动扫描注册
        RegisterQueriesByConvention(services);

        return services;
    }
    
    /// <summary>
    /// 按约定自动注册查询服务
    /// </summary>
    /// <param name="services">服务集合</param>
    private static void RegisterQueriesByConvention(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // 获取所有查询接口和实现类
        var queryTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Queries"))
            .ToList();
            
        foreach (var implementationType in queryTypes)
        {
            // 查找对应的接口
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");
                
            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
    }
    
    /// <summary>
    /// 添加 Mapster 对象映射配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        // 扫描并注册 Mapster 配置
        Mapster.TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfiguration).Assembly);

        return services;
    }
}
