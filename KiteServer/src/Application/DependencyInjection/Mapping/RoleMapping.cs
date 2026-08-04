namespace Application.DependencyInjection.Mapping;

/// <summary>
/// 角色模块映射配置
/// </summary>
public class RoleMapping : IRegister
{
    /// <summary>
    /// 注册角色模块对象映射配置
    /// </summary>
    /// <param name="config">类型适配器配置</param>
    public void Register(TypeAdapterConfig config)
    {
        // 配置Role实体到RoleDto的映射
        config.ForType<Role, RoleDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RoleName, src => src.RoleName)
            .Map(dest => dest.RoleCode, src => src.RoleCode)
            .Map(dest => dest.Sort, src => src.Sort)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DataScope, src => (int)src.DataScope)
            .Map(dest => dest.Remark, src => src.Remark)
            .Map(dest => dest.CreateTime, src => src.CreateTime)
            .Map(dest => dest.UpdateTime, src => src.UpdateTime);
    }
}
