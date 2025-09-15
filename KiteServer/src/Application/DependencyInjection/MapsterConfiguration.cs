using Mapster;
using Domain.Entities;
using Shared.Models.User;

namespace Application.DependencyInjection;

/// <summary>
/// Mapster配置
/// </summary>
public class MapsterConfiguration : IRegister
{
    /// <summary>
    /// 注册对象映射配置
    /// </summary>
    /// <param name="config">类型适配器配置</param>
    public void Register(TypeAdapterConfig config)
    {
        // 配置User实体到UserDto的映射
        config.ForType<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.RealName, src => src.RealName)
            .Map(dest => dest.DingTalkId, src => src.DingTalkId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Remark, src => src.Remark)
            .Map(dest => dest.CreateTime, src => src.CreateTime)
            .Ignore(dest => dest.UpdateTime); // 忽略密码等敏感字段

        // 配置CreateUserCommand到User实体的映射
        config.ForType<Application.Commands.User.CreateUserCommand, User>()
            .Map(dest => dest.UserName, src => src.UserName)
            .Map(dest => dest.Password, src => src.Password)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.RealName, src => src.RealName)
            .Map(dest => dest.DingTalkId, src => src.DingTalkId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Remark, src => src.Remark)
            .Ignore(dest => dest.Id) // ID由数据库生成
            .Ignore(dest => dest.CreateTime) // 由AOP自动设置
            .Ignore(dest => dest.UpdateTime) // 由AOP自动设置
            .Ignore(dest => dest.IsDeleted); // 由AOP自动设置

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

        // 全局配置
        config.Default.PreserveReference(true); // 保持引用关系
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible); // 灵活的名称匹配策略
    }
}
