using Application.DependencyInjection.Mapping;
using Domain.Entities;
using Mapster;
using Shared.Enums;
using Shared.Models.Dtos;

namespace UnitTests.DependencyInjection.Mapping;

/// <summary>
/// <see cref="RoleMapping"/> 单元测试
/// </summary>
public class RoleMappingTests
{
    /// <summary>
    /// 创建仅包含角色模块映射规则的配置，避免污染全局配置
    /// </summary>
    private static TypeAdapterConfig CreateConfig()
    {
        var config = new TypeAdapterConfig();
        new RoleMapping().Register(config);
        return config;
    }

    [Fact(DisplayName = "Role实体映射到RoleDto：各字段正确映射且数据权限范围转为int")]
    public void Adapt_RoleToRoleDto_MapsFieldsAndConvertsDataScope()
    {
        // 准备
        var config = CreateConfig();
        var createTime = new DateTime(2026, 3, 1, 9, 0, 0);
        var updateTime = new DateTime(2026, 3, 2, 10, 0, 0);
        var role = new Role
        {
            Id = 1,
            RoleName = "管理员",
            RoleCode = "admin",
            Sort = 1,
            Status = 1,
            DataScope = DataScope.Department,
            Remark = "系统内置角色",
            CreateTime = createTime,
            UpdateTime = updateTime
        };

        // 执行
        var dto = role.Adapt<RoleDto>(config);

        // 断言
        Assert.Equal(1, dto.Id);
        Assert.Equal("管理员", dto.RoleName);
        Assert.Equal("admin", dto.RoleCode);
        Assert.Equal(1, dto.Sort);
        Assert.Equal(1, dto.Status);
        Assert.Equal((int)DataScope.Department, dto.DataScope); // 枚举转为int
        Assert.Equal("系统内置角色", dto.Remark);
        Assert.Equal(createTime, dto.CreateTime);
        Assert.Equal(updateTime, dto.UpdateTime);
    }
}
