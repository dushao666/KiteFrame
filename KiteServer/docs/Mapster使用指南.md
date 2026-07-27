# Mapster 对象映射使用指南

## 概述

KiteServer 项目使用 Mapster 作为对象映射工具，替代了之前的 AutoMapper。Mapster 具有更好的性能和更简洁的API。

## 配置

### 1. 全局配置

在 `Application/DependencyInjection/MapsterConfiguration.cs` 中配置映射规则：

```csharp
public class MapsterConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // 配置User实体到UserDto的映射
        config.ForType<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserName, src => src.UserName)
            .Ignore(dest => dest.Password); // 忽略敏感字段
        
        // 全局配置
        config.Default.PreserveReference(true);
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
    }
}
```

### 2. 服务注册

在 `Api/Extensions/ProgramExtensions.cs` 中注册配置：

```csharp
private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
{
    // 扫描并注册 Mapster 配置
    Mapster.TypeAdapterConfig.GlobalSettings.Scan(typeof(Application.DependencyInjection.MapsterConfiguration).Assembly);
    return services;
}
```

## 基本使用

### 1. 单个对象映射

```csharp
// 将User实体映射为UserDto
var user = await context.Users.GetByIdAsync(id);
var userDto = user.Adapt<UserDto>();

// 将CreateUserCommand映射为User实体
var user = request.Adapt<Domain.Entities.User>();
```

### 2. 集合映射

```csharp
// 将User实体列表映射为UserDto列表
var users = await query.ToListAsync();
var userDtos = users.Adapt<List<UserDto>>();
```

### 3. 条件映射

```csharp
// 根据条件进行不同的映射
config.ForType<User, UserDto>()
    .Map(dest => dest.StatusText, 
         src => src.Status == 1 ? "启用" : "禁用");
```

## 高级功能

### 1. 忽略字段

```csharp
config.ForType<User, UserDto>()
    .Ignore(dest => dest.Password)
    .Ignore(dest => dest.CreateUserId);
```

### 2. 自定义映射

```csharp
config.ForType<User, UserDto>()
    .Map(dest => dest.FullName, 
         src => $"{src.RealName}({src.UserName})");
```

### 3. 嵌套对象映射

```csharp
config.ForType<Order, OrderDto>()
    .Map(dest => dest.User, src => src.User.Adapt<UserDto>());
```

## 性能优化

### 1. 编译时映射

Mapster 支持编译时生成映射代码，性能更好：

```csharp
// 使用编译时映射
var userDto = user.Adapt<UserDto>();
```

### 2. 映射缓存

Mapster 会自动缓存映射配置，避免重复解析。

## 最佳实践

### 1. 统一配置

- 所有映射配置都放在 `MapsterConfiguration` 类中
- 使用有意义的映射规则名称
- 为复杂映射添加注释

### 2. 安全考虑

- 忽略敏感字段（如密码）
- 验证映射结果
- 处理空值情况

### 3. 性能考虑

- 优先使用 `Adapt<T>()` 方法
- 避免在循环中重复配置映射
- 使用批量映射处理集合

## 示例代码

### UserQueries 中的使用

```csharp
public async Task<ApiResult<UserDto>> GetUserByIdAsync(long id)
{
    var user = await context.Users.GetByIdAsync(id);
    if (user == null)
        return ApiResult<UserDto>.Fail("用户不存在");

    // 使用Mapster进行对象映射
    var userDto = user.Adapt<UserDto>();
    return ApiResult<UserDto>.Ok(userDto, "获取成功");
}
```

### CreateUserCommandHandler 中的使用

```csharp
public async Task<ApiResult<long>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
{
    // 使用Mapster进行对象映射
    var user = request.Adapt<Domain.Entities.User>();
    
    var result = await context.Users.InsertReturnEntityAsync(user);
    return ApiResult<long>.Ok(result.Id, "创建成功");
}
```

## 迁移说明

从 AutoMapper 迁移到 Mapster：

1. 移除 AutoMapper 相关包引用
2. 添加 Mapster 包引用
3. 更新 `_Imports.cs` 文件
4. 重写映射配置
5. 更新使用映射的代码

## 参考资源

- [Mapster 官方文档](https://github.com/MapsterMapper/Mapster)
- [Mapster 性能对比](https://github.com/MapsterMapper/Mapster/wiki/Benchmark)
