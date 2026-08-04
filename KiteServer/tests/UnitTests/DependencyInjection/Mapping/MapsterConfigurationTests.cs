namespace UnitTests.DependencyInjection.Mapping;

/// <summary>
/// <see cref="MapsterConfiguration"/> 全局映射配置单元测试
/// </summary>
public class MapsterConfigurationTests
{
    /// <summary>
    /// 测试用引用对象
    /// </summary>
    private sealed class RefItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 测试用包含重复引用的容器对象
    /// </summary>
    private sealed class RefContainer
    {
        /// <summary>
        /// 第一个引用
        /// </summary>
        public RefItem? First { get; set; }

        /// <summary>
        /// 第二个引用
        /// </summary>
        public RefItem? Second { get; set; }
    }

    /// <summary>
    /// 测试用源对象（属性名含下划线，命名风格与目标不同）
    /// </summary>
    private sealed class FlexibleSource
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string User_Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 测试用目标对象（属性名为标准驼峰命名）
    /// </summary>
    private sealed class FlexibleDestination
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
    }

    [Fact(DisplayName = "全局配置：开启引用保持后，重复引用的对象映射后仍为同一实例")]
    public void Register_PreserveReference_KeepsSharedReference()
    {
        // 准备
        var config = new TypeAdapterConfig();
        new MapsterConfiguration().Register(config);
        var shared = new RefItem { Name = "共享对象" };
        var container = new RefContainer { First = shared, Second = shared };

        // 执行
        var mapped = container.Adapt<RefContainer>(config);

        // 断言：两个属性指向同一映射实例（引用关系被保持）
        Assert.NotNull(mapped.First);
        Assert.Same(mapped.First, mapped.Second);
        Assert.Equal("共享对象", mapped.First!.Name);
    }

    [Fact(DisplayName = "全局配置：灵活名称匹配策略下，下划线分隔的属性名也能映射到驼峰属性")]
    public void Register_FlexibleNameMatching_MapsUnderscoreSeparatedProperties()
    {
        // 准备
        var config = new TypeAdapterConfig();
        new MapsterConfiguration().Register(config);
        var source = new FlexibleSource { User_Name = "admin" };

        // 执行
        var destination = source.Adapt<FlexibleDestination>(config);

        // 断言：User_Name -> UserName 按灵活匹配策略完成映射
        Assert.Equal("admin", destination.UserName);
    }
}
