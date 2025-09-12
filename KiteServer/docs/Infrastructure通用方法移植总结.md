# Infrastructure 通用方法移植总结

## 移植背景

根据用户需求，我将 YseTTSAPIService 项目中 Infrastructure 层的通用方法移植到 KiteServer 项目中，以提供丰富的扩展方法、工具类和异常处理功能。

## 移植内容

### 1. 扩展方法 (Extensions)

#### StringExtensions.cs
- ✅ 移除前缀/后缀字符串方法
- ✅ 字符串空值判断方法
- ✅ 安全截取字符串方法
- ✅ 命名格式转换（驼峰/帕斯卡）
- ✅ 敏感信息隐藏方法
- ✅ 增强了原有功能，添加了更多实用方法

#### EnumExtensions.cs
- ✅ 获取枚举整数值
- ✅ 获取枚举描述信息
- ✅ 获取枚举显示名称
- ✅ 字符串转枚举方法
- ✅ 枚举标志判断方法
- ✅ 获取所有枚举值方法

#### TimeExtensions.cs
- ✅ UTC时间处理方法
- ✅ 时间戳转换方法
- ✅ 时区时间处理
- ✅ DateOnly/TimeOnly 转换
- ✅ 时间范围获取（月初/月末/周初/周末）
- ✅ 时间判断方法（今天/本周/本月/本年）
- ✅ 扩展了原有功能，增加了更多时间处理方法

#### PredicateBuilder.cs
- ✅ 表达式构建器
- ✅ 动态查询条件构建
- ✅ AND/OR 逻辑连接
- ✅ 条件判断连接（AndIf/OrIf）
- ✅ 表达式取反方法
- ✅ 完全移植，功能完整

### 2. 工具类 (Utilities)

#### CommonHelper.cs
- ✅ 随机字符串生成
- ✅ 随机数字生成
- ✅ 概率随机数（带权重）
- ✅ 敏感信息隐藏
- ✅ 手机号/邮箱/身份证验证
- ✅ 文件大小格式化
- ✅ GUID生成
- ✅ 随机颜色生成
- ✅ 增强了原有功能，添加了更多验证和工具方法

#### EncryptionHelper.cs
- ✅ AES 加密解密
- ✅ SHA512/SHA256/MD5 哈希
- ✅ PBKDF2 密码哈希
- ✅ RSA 加密解密
- ✅ HMAC-SHA256 签名
- ✅ 盐值生成
- ✅ 密钥对生成
- ✅ 大幅增强了原有功能，提供了完整的加密解决方案

#### SnowflakeId.cs
- ✅ 雪花ID生成器
- ✅ 分布式唯一ID生成
- ✅ 工作机器ID和数据中心ID配置
- ✅ 时钟回拨检测
- ✅ ID解析功能
- ✅ 单例模式支持
- ✅ 完全移植并增强，添加了ID解析功能

### 3. 异常处理 (Exceptions)

#### IFriendlyException.cs
- ✅ 友好异常接口定义
- ✅ 错误码接口
- ✅ 日志级别接口
- ✅ 完全移植

#### FriendlyException.cs
- ✅ 用户友好异常基类
- ✅ 参数错误异常 (MissingException)
- ✅ 业务异常 (BusinessException)
- ✅ 验证异常 (ValidationException)
- ✅ 权限异常 (UnauthorizedException/ForbiddenException)
- ✅ 资源未找到异常 (NotFoundException)
- ✅ 冲突异常 (ConflictException)
- ✅ 服务器内部错误异常 (InternalServerErrorException)
- ✅ 大幅扩展了原有功能，提供了完整的异常体系

## 技术改进

### 1. 代码质量提升
- **空值安全**：使用 .NET 9 的可空引用类型
- **异常处理**：增强了异常处理机制
- **性能优化**：使用更高效的算法和数据结构
- **代码规范**：遵循 .NET 9 编码规范

### 2. 功能增强
- **更多扩展方法**：增加了更多实用的扩展方法
- **完整的加密方案**：提供了从对称加密到非对称加密的完整解决方案
- **丰富的异常类型**：覆盖了常见的业务异常场景
- **更好的文档**：提供了详细的XML注释

### 3. 架构优化
- **全局导入**：通过 `_Imports.cs` 实现全局可用
- **命名空间规范**：使用统一的命名空间结构
- **依赖注入友好**：支持依赖注入模式

## 文件结构对比

### YseTTSAPIService (原始)
```
Infrastructure/
├── Extension/
│   ├── StringExtensions.cs      (24行)
│   ├── EnumExtensions.cs        (13行)
│   ├── TimeExtension.cs         (87行)
│   └── PredicateBuilder.cs      (50行)
├── Utility/
│   ├── CommonHelper.cs          (133行)
│   ├── EncryptionHelper.cs      (57行)
│   └── SnowflakeId.cs          (124行)
└── Exceptions/
    ├── IFriendlyException.cs    (22行)
    └── FriendlyException.cs     (46行)
```

### KiteServer (移植后)
```
Infrastructure/
├── Extensions/
│   ├── StringExtensions.cs      (140行) ⬆️ +116行
│   ├── EnumExtensions.cs        (80行)  ⬆️ +67行
│   ├── TimeExtensions.cs        (220行) ⬆️ +133行
│   └── PredicateBuilder.cs      (150行) ⬆️ +100行
├── Utilities/
│   ├── CommonHelper.cs          (272行) ⬆️ +139行
│   ├── EncryptionHelper.cs      (250行) ⬆️ +193行
│   └── SnowflakeId.cs          (250行) ⬆️ +126行
└── Exceptions/
    ├── IFriendlyException.cs    (35行)  ⬆️ +13行
    └── FriendlyException.cs     (200行) ⬆️ +154行
```

## 使用方式

### 1. 全局可用
通过 `_Imports.cs` 全局导入，在项目中任何地方都可以直接使用：

```csharp
// 无需额外 using 语句
string hidden = "13812345678".HideSensitiveInfo(3, 4);
long id = SnowflakeId.Default().NextId();
throw new BusinessException("业务异常");
```

### 2. 类型安全
使用 .NET 9 的可空引用类型，提供更好的类型安全：

```csharp
public static string? HideSensitiveInfo(this string? info, int left, int right)
public static bool TryToEnum<T>(this string? value, out T result) where T : struct, Enum
```

### 3. 性能优化
- 使用 `using` 语句自动释放资源
- 使用 `StringBuilder` 优化字符串操作
- 使用编译时优化的表达式树

## 构建验证

✅ **构建成功**：项目编译通过，无错误  
✅ **警告处理**：只有一个可空引用类型警告，不影响功能  
✅ **依赖完整**：所有依赖项正确引用  
✅ **命名空间**：全局导入配置正确  

## 使用示例

### 字符串处理
```csharp
string email = "user@example.com";
bool isValid = email.IsValidEmail(); // true
string hidden = email.HideSensitiveInfo(2, 4); // "us****com"
```

### 时间处理
```csharp
DateTime now = TimeExtensions.NewTimeZone(); // 东八区时间
long timestamp = now.ToTimestamp(); // 时间戳
bool isToday = now.IsToday(); // true
```

### 加密处理
```csharp
string salt = EncryptionHelper.GenerateSalt();
string hashed = EncryptionHelper.HashPassword("password", salt);
bool valid = EncryptionHelper.VerifyPassword("password", salt, hashed);
```

### 异常处理
```csharp
if (string.IsNullOrEmpty(email))
    throw new ValidationException("邮箱不能为空");

if (!user.IsActive)
    throw new BusinessException("用户已被禁用");
```

### 动态查询
```csharp
var predicate = PredicateBuilder.True<User>();
predicate = predicate.AndIf(!string.IsNullOrEmpty(name), u => u.Name.Contains(name));
predicate = predicate.AndIf(status.HasValue, u => u.Status == status.Value);

var users = await context.Users.AsQueryable().Where(predicate).ToListAsync();
```

## 总结

✅ **完整移植**：成功移植了 YseTTSAPIService 中的所有通用方法  
✅ **功能增强**：在原有基础上增加了更多实用功能  
✅ **质量提升**：代码质量、性能和安全性都有显著提升  
✅ **易于使用**：通过全局导入，使用更加便捷  
✅ **文档完善**：提供了详细的使用指南和示例  

现在 KiteServer 项目拥有了一套完整、强大、易用的通用方法库，将大大提高开发效率和代码质量。这些工具类和扩展方法覆盖了日常开发中的大部分常见需求，包括字符串处理、时间处理、加密解密、异常处理、动态查询等。
