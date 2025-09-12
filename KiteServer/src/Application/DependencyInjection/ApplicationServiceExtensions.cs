using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Queries.User;

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
        
        // 添加命令处理器（MediatR会自动扫描）
        // 这里不需要手动注册，MediatR会自动发现和注册
        
        return services;
    }
    
    /// <summary>
    /// 添加查询服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    private static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        // 自动扫描注册查询服务
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
