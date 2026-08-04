using Application.DependencyInjection.Mapping;
using Domain.Entities;
using Mapster;
using Shared.Enums;
using Shared.Models.Dtos;

namespace UnitTests.DependencyInjection.Mapping;

/// <summary>
/// <see cref="MenuMapping"/> 单元测试
/// </summary>
public class MenuMappingTests
{
    /// <summary>
    /// 创建仅包含菜单模块映射规则的配置，避免污染全局配置
    /// </summary>
    private static TypeAdapterConfig CreateConfig()
    {
        var config = new TypeAdapterConfig();
        new MenuMapping().Register(config);
        return config;
    }

    [Fact(DisplayName = "Menu实体映射到MenuDto：各字段正确映射且菜单类型枚举值保持不变")]
    public void Adapt_MenuToMenuDto_MapsFieldsAndPreservesMenuType()
    {
        // 准备
        var config = CreateConfig();
        var menu = new Menu
        {
            Id = 10,
            ParentId = 1,
            MenuName = "系统管理",
            MenuCode = "system",
            MenuType = MenuType.Directory,
            Path = "/system",
            Component = "Layout",
            Icon = "setting",
            Sort = 5,
            IsVisible = true,
            IsCache = true,
            IsFrame = false,
            Status = 1,
            Permissions = "system:view,system:edit",
            Remark = "一级目录"
        };

        // 执行
        var dto = menu.Adapt<MenuDto>(config);

        // 断言
        Assert.Equal(10, dto.Id);
        Assert.Equal(1, dto.ParentId);
        Assert.Equal("系统管理", dto.MenuName);
        Assert.Equal("system", dto.MenuCode);
        Assert.Equal(MenuType.Directory, dto.MenuType); // 经int转换后枚举值保持不变
        Assert.Equal("/system", dto.Path);
        Assert.Equal("Layout", dto.Component);
        Assert.Equal("setting", dto.Icon);
        Assert.Equal(5, dto.Sort);
        Assert.True(dto.IsVisible);
        Assert.True(dto.IsCache);
        Assert.False(dto.IsFrame);
        Assert.Equal(1, dto.Status);
        Assert.Equal("system:view,system:edit", dto.Permissions);
        Assert.Equal("一级目录", dto.Remark);
    }

    [Fact(DisplayName = "Menu实体映射到MenuDto：子菜单由业务逻辑处理，映射时被忽略")]
    public void Adapt_MenuToMenuDto_IgnoresChildren()
    {
        // 准备
        var config = CreateConfig();
        var menu = new Menu
        {
            Id = 1,
            MenuName = "父菜单",
            MenuCode = "parent",
            Children = new List<Menu>
            {
                new Menu { Id = 2, ParentId = 1, MenuName = "子菜单", MenuCode = "child" }
            }
        };

        // 执行
        var dto = menu.Adapt<MenuDto>(config);

        // 断言：Children被忽略，保持默认空集合
        Assert.NotNull(dto.Children);
        Assert.Empty(dto.Children);
    }
}
