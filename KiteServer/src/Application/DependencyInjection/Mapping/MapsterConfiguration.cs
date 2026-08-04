using Mapster;

namespace Application.DependencyInjection.Mapping;

/// <summary>
/// Mapster全局映射配置
/// </summary>
/// <remarks>
/// 各业务模块的映射规则按模块拆分，位于本目录下的 {模块}Mapping 类（均实现 <see cref="IRegister"/>），
/// 由 <c>TypeAdapterConfig.GlobalSettings.Scan(...)</c> 程序集扫描统一加载；本类仅负责全局默认配置。
/// </remarks>
public class MapsterConfiguration : IRegister
{
    /// <summary>
    /// 注册全局默认映射配置
    /// </summary>
    /// <param name="config">类型适配器配置</param>
    public void Register(TypeAdapterConfig config)
    {
        // 全局配置
        config.Default.PreserveReference(true); // 保持引用关系
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible); // 灵活的名称匹配策略
    }
}
