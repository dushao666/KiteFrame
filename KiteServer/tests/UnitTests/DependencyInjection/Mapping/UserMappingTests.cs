namespace UnitTests.DependencyInjection.Mapping;

/// <summary>
/// <see cref="UserMapping"/> 单元测试
/// </summary>
public class UserMappingTests
{
    /// <summary>
    /// 创建仅包含用户模块映射规则的配置，避免污染全局配置
    /// </summary>
    private static TypeAdapterConfig CreateConfig()
    {
        var config = new TypeAdapterConfig();
        new UserMapping().Register(config);
        return config;
    }

    [Fact(DisplayName = "User实体映射到UserDto：各字段正确映射且UpdateTime被忽略")]
    public void Adapt_UserToUserDto_MapsFieldsAndIgnoresUpdateTime()
    {
        // 准备
        var config = CreateConfig();
        var createTime = new DateTime(2026, 1, 1, 10, 30, 0);
        var updateTime = new DateTime(2026, 2, 1, 8, 0, 0);
        var user = new User
        {
            Id = 100,
            UserName = "admin",
            Password = "secret-hash",
            Email = "admin@example.com",
            Phone = "13800138000",
            RealName = "管理员",
            DingTalkId = "dingtalk-001",
            Status = 1,
            Remark = "系统管理员",
            CreateTime = createTime,
            UpdateTime = updateTime
        };

        // 执行
        var dto = user.Adapt<UserDto>(config);

        // 断言
        Assert.Equal(100, dto.Id);
        Assert.Equal("admin", dto.UserName);
        Assert.Equal("admin@example.com", dto.Email);
        Assert.Equal("13800138000", dto.Phone);
        Assert.Equal("管理员", dto.RealName);
        Assert.Equal("dingtalk-001", dto.DingTalkId);
        Assert.Equal(1, dto.Status);
        Assert.Equal("系统管理员", dto.Remark);
        Assert.Equal(createTime, dto.CreateTime);
        Assert.Equal(default, dto.UpdateTime); // UpdateTime被显式忽略
    }

    [Fact(DisplayName = "CreateUserCommand映射到User实体：业务字段映射且审计字段被忽略")]
    public void Adapt_CreateUserCommandToUser_MapsBusinessFieldsAndIgnoresAuditFields()
    {
        // 准备
        var config = CreateConfig();
        var command = new CreateUserCommand
        {
            UserName = "zhangsan",
            Password = "P@ssw0rd",
            Email = "zhangsan@example.com",
            Phone = "13900139000",
            RealName = "张三",
            DingTalkId = "dingtalk-002",
            Status = 1,
            Remark = "普通用户"
        };

        // 执行
        var user = command.Adapt<User>(config);

        // 断言：业务字段正确映射
        Assert.Equal("zhangsan", user.UserName);
        Assert.Equal("P@ssw0rd", user.Password);
        Assert.Equal("zhangsan@example.com", user.Email);
        Assert.Equal("13900139000", user.Phone);
        Assert.Equal("张三", user.RealName);
        Assert.Equal("dingtalk-002", user.DingTalkId);
        Assert.Equal(1, user.Status);
        Assert.Equal("普通用户", user.Remark);

        // 断言：审计与主键字段被忽略，保持默认值（由数据库或AOP填充）
        Assert.Equal(0, user.Id);
        Assert.Equal(default, user.CreateTime);
        Assert.Equal(default, user.UpdateTime);
        Assert.False(user.IsDeleted);
    }
}
