using Mapster;
using Domain.Entities;
using Shared.Models.Dtos;

namespace Application.DependencyInjection.Mapping;

/// <summary>
/// 菜单模块映射配置
/// </summary>
public class MenuMapping : IRegister
{
    /// <summary>
    /// 注册菜单模块对象映射配置
    /// </summary>
    /// <param name="config">类型适配器配置</param>
    public void Register(TypeAdapterConfig config)
    {
        // 配置Menu实体到MenuDto的映射
        config.ForType<Menu, MenuDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.MenuName, src => src.MenuName)
            .Map(dest => dest.MenuCode, src => src.MenuCode)
            .Map(dest => dest.MenuType, src => (int)src.MenuType)
            .Map(dest => dest.Path, src => src.Path)
            .Map(dest => dest.Component, src => src.Component)
            .Map(dest => dest.Icon, src => src.Icon)
            .Map(dest => dest.Sort, src => src.Sort)
            .Map(dest => dest.IsVisible, src => src.IsVisible)
            .Map(dest => dest.IsCache, src => src.IsCache)
            .Map(dest => dest.IsFrame, src => src.IsFrame)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Permissions, src => src.Permissions)
            .Map(dest => dest.Remark, src => src.Remark)
            .Ignore(dest => dest.Children); // 子菜单由业务逻辑处理
    }
}
