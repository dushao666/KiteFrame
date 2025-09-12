# Infrastructure 通用方法使用指南

## 概述

KiteServer 项目的 Infrastructure 层现在包含了从 YseTTSAPIService 项目移植过来的通用方法和工具类，提供了丰富的扩展方法、工具类和异常处理功能。

## 目录结构

```
Infrastructure/
├── Extensions/                    # 扩展方法
│   ├── StringExtensions.cs       # 字符串扩展方法
│   ├── EnumExtensions.cs         # 枚举扩展方法
│   ├── TimeExtensions.cs         # 时间扩展方法
│   └── PredicateBuilder.cs       # 表达式构建器
├── Utilities/                     # 工具类
│   ├── CommonHelper.cs           # 通用帮助类
│   ├── EncryptionHelper.cs       # 加密帮助类
│   └── SnowflakeId.cs           # 雪花ID生成器
├── Exceptions/                    # 异常处理
│   ├── IFriendlyException.cs     # 友好异常接口
│   └── FriendlyException.cs      # 友好异常实现
└── Services/                      # 服务
    └── CurrentUserService.cs     # 当前用户服务
```

## 扩展方法

### 字符串扩展 (StringExtensions)

```csharp
// 移除末尾指定字符串
string result = "HelloWorld".RemovePostFix("World"); // "Hello"

// 移除开头指定字符串
string result = "HelloWorld".RemovePreFix("Hello"); // "World"

// 判断字符串是否为空
bool isEmpty = "".IsNullOrWhiteSpace(); // true
bool isNotEmpty = "Hello".IsNotNullOrWhiteSpace(); // true

// 安全截取字符串
string truncated = "这是一个很长的字符串".Truncate(5); // "这是一个很..."

// 转换命名格式
string camelCase = "HelloWorld".ToCamelCase(); // "helloWorld"
string pascalCase = "helloWorld".ToPascalCase(); // "HelloWorld"

// 隐藏敏感信息
string hidden = "13812345678".HideSensitiveInfo(3, 4); // "138****5678"
```

### 枚举扩展 (EnumExtensions)

```csharp
public enum Status
{
    [Description("激活状态")]
    Active = 1,
    
    [Description("禁用状态")]
    Inactive = 0
}

// 获取枚举值
int value = Status.Active.GetValue(); // 1

// 获取描述信息
string desc = Status.Active.GetDescription(); // "激活状态"

// 字符串转枚举
Status status = "Active".ToEnum<Status>();

// 尝试转换枚举
if ("Active".TryToEnum<Status>(out Status result))
{
    // 转换成功
}

// 获取所有枚举值
var allValues = EnumExtensions.GetValues<Status>();
```

### 时间扩展 (TimeExtensions)

```csharp
// 获取当前时区时间（东八区）
DateTime now = TimeExtensions.NewTimeZone();

// 时间戳转换
DateTime dateTime = TimeExtensions.ConvertTimestamp(1640995200); // 2022-01-01 08:00:00
long timestamp = DateTime.Now.ToTimestamp(); // 当前时间戳（秒）
long milliTimestamp = DateTime.Now.ToMilliTimestamp(); // 当前时间戳（毫秒）

// 日期判断
bool isToday = DateTime.Now.IsToday(); // true
bool isThisWeek = DateTime.Now.IsThisWeek(); // true
bool isThisMonth = DateTime.Now.IsThisMonth(); // true

// 获取时间范围
DateTime startOfMonth = DateTime.Now.StartOfMonth(); // 本月1号
DateTime endOfMonth = DateTime.Now.EndOfMonth(); // 本月最后一天
DateTime startOfWeek = DateTime.Now.StartOfWeek(); // 本周一
DateTime endOfWeek = DateTime.Now.EndOfWeek(); // 本周日

// DateOnly 和 TimeOnly 转换
DateOnly? date = TimeExtensions.DateOnlyFormat(DateTime.Now);
TimeOnly? time = TimeExtensions.TimeOnlyFormat(DateTime.Now);
DateTime? combined = TimeExtensions.DateTimeFormat(date, time);
```

### 表达式构建器 (PredicateBuilder)

```csharp
// 动态构建查询条件
var predicate = PredicateBuilder.True<User>();

if (!string.IsNullOrEmpty(name))
{
    predicate = predicate.And(u => u.Name.Contains(name));
}

if (status.HasValue)
{
    predicate = predicate.And(u => u.Status == status.Value);
}

// 使用条件构建
predicate = predicate.AndIf(!string.IsNullOrEmpty(email), u => u.Email == email);

// 在查询中使用
var users = await context.Users.AsQueryable().Where(predicate).ToListAsync();
```

## 工具类

### 通用帮助类 (CommonHelper)

```csharp
// 生成随机字符串
string randomStr = CommonHelper.GenerateRandom(8); // "A1B2C3D4"
string randomNum = CommonHelper.GenerateRandomNumber(6); // "123456"

// 生成GUID
string guid = CommonHelper.GenerateGuid(); // "32位GUID"
string guidWithFormat = CommonHelper.GenerateGuid("D"); // "带连字符的GUID"

// 验证格式
bool isValidMobile = CommonHelper.IsValidMobile("13812345678"); // true
bool isValidEmail = CommonHelper.IsValidEmail("test@example.com"); // true
bool isValidIdCard = CommonHelper.IsValidIdCard("身份证号"); // true/false

// 文件大小格式化
string fileSize = CommonHelper.FormatFileSize(1024 * 1024); // "1 MB"

// 隐藏敏感信息
string hiddenInfo = CommonHelper.HideSensitiveInfo("13812345678", 3, 4); // "138****5678"

// 获取随机颜色
string color = CommonHelper.GetRandomColor(); // "#FF5733"
```

### 加密帮助类 (EncryptionHelper)

```csharp
// AES 加密解密
string key = "12345678901234567890123456789012"; // 32字节密钥
string encrypted = EncryptionHelper.EncryptBytes("Hello World", key);
string decrypted = EncryptionHelper.DecryptBytes(encrypted, key);

// 哈希算法
string md5 = EncryptionHelper.Md5("Hello World");
string sha256 = EncryptionHelper.Sha256("Hello World");
string sha512 = EncryptionHelper.Sha512("Hello World");

// 密码哈希（PBKDF2）
string salt = EncryptionHelper.GenerateSalt();
string hashedPassword = EncryptionHelper.HashPassword("password123", salt);
bool isValid = EncryptionHelper.VerifyPassword("password123", salt, hashedPassword);

// RSA 加密解密
var (publicKey, privateKey) = EncryptionHelper.GenerateRsaKeyPair();
string rsaEncrypted = EncryptionHelper.RsaEncrypt("Hello World", publicKey);
string rsaDecrypted = EncryptionHelper.RsaDecrypt(rsaEncrypted, privateKey);

// HMAC 签名
string signature = EncryptionHelper.HmacSha256("data", "secret");
bool isValidSignature = EncryptionHelper.VerifyHmacSha256("data", "secret", signature);
```

### 雪花ID生成器 (SnowflakeId)

```csharp
// 获取默认实例
var snowflake = SnowflakeId.Default();

// 生成ID
long id = snowflake.NextId(); // 19位长整型ID
string idString = snowflake.NextIdString(); // 字符串格式ID

// 创建自定义实例
var customSnowflake = new SnowflakeId(workerId: 1, datacenterId: 1);
long customId = customSnowflake.NextId();

// 解析雪花ID
var idInfo = SnowflakeId.ParseId(id);
Console.WriteLine($"时间: {idInfo.DateTime}");
Console.WriteLine($"工作机器ID: {idInfo.WorkerId}");
Console.WriteLine($"数据中心ID: {idInfo.DatacenterId}");
Console.WriteLine($"序列号: {idInfo.Sequence}");
```

## 异常处理

### 友好异常类

```csharp
// 业务异常
throw new BusinessException("业务处理失败");
throw new BusinessException(1001, "自定义错误码的业务异常");

// 验证异常
throw new ValidationException("验证失败");
var errors = new Dictionary<string, string[]>
{
    ["Name"] = new[] { "名称不能为空" },
    ["Email"] = new[] { "邮箱格式不正确" }
};
throw new ValidationException(errors);

// 资源未找到异常
throw new NotFoundException("用户未找到");
throw new NotFoundException("User", userId);

// 权限异常
throw new UnauthorizedException("未登录");
throw new ForbiddenException("权限不足");

// 冲突异常
throw new ConflictException("用户名已存在");

// 服务器内部错误
throw new InternalServerErrorException("服务器内部错误", innerException);
```

## 在项目中使用

### 1. 全局导入

由于在 `_Imports.cs` 中已经添加了全局导入，所以在项目中可以直接使用这些扩展方法和工具类：

```csharp
// 在任何 .cs 文件中直接使用，无需额外的 using 语句
public class UserService
{
    public async Task<string> CreateUserAsync(string name, string email)
    {
        // 验证邮箱格式
        if (!email.IsValidEmail())
        {
            throw new ValidationException("邮箱格式不正确");
        }
        
        // 生成用户ID
        var snowflake = SnowflakeId.Default();
        long userId = snowflake.NextId();
        
        // 隐藏敏感信息用于日志
        string hiddenEmail = email.HideSensitiveInfo(2, 4);
        _logger.LogInformation($"创建用户: {name}, 邮箱: {hiddenEmail}");
        
        return userId.ToString();
    }
}
```

### 2. 在查询中使用

```csharp
public class UserQueries : IUserQueries
{
    public async Task<List<UserDto>> SearchUsersAsync(UserSearchRequest request)
    {
        using var context = _unitOfWork.CreateContext(false);
        
        // 使用 PredicateBuilder 构建动态查询条件
        var predicate = PredicateBuilder.True<User>();
        
        predicate = predicate.AndIf(request.Name.IsNotNullOrWhiteSpace(), 
            u => u.Name.Contains(request.Name));
        
        predicate = predicate.AndIf(request.Status.HasValue, 
            u => u.Status == request.Status.Value);
        
        predicate = predicate.AndIf(request.StartDate.HasValue, 
            u => u.CreateTime >= request.StartDate.Value);
        
        var users = await context.Users.AsQueryable()
            .Where(predicate)
            .ToListAsync();
            
        return users.Adapt<List<UserDto>>();
    }
}
```

### 3. 在控制器中使用

```csharp
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task<ApiResult<string>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            // 验证手机号格式
            if (!request.Mobile.IsValidMobile())
            {
                throw new ValidationException("手机号格式不正确");
            }
            
            // 生成随机密码
            string randomPassword = CommonHelper.GenerateRandom(8);
            
            // 加密密码
            string salt = EncryptionHelper.GenerateSalt();
            string hashedPassword = EncryptionHelper.HashPassword(randomPassword, salt);
            
            // 创建用户逻辑...
            
            return ApiResult<string>.Ok("创建成功");
        }
        catch (ValidationException ex)
        {
            return ApiResult<string>.Fail(ex.Message);
        }
        catch (BusinessException ex)
        {
            return ApiResult<string>.Fail(ex.Message);
        }
    }
}
```

## 最佳实践

1. **异常处理**：使用友好异常类替代普通异常，提供更好的错误信息
2. **参数验证**：使用扩展方法进行常见格式验证
3. **敏感信息**：在日志记录时使用 `HideSensitiveInfo` 隐藏敏感信息
4. **动态查询**：使用 `PredicateBuilder` 构建复杂的动态查询条件
5. **ID生成**：使用雪花ID生成器生成分布式唯一ID
6. **加密安全**：使用 `EncryptionHelper` 进行数据加密和密码哈希

## 性能考虑

1. **雪花ID生成器**：单例模式，线程安全，高性能
2. **表达式构建器**：编译时优化，性能优于反射
3. **加密算法**：使用 .NET 原生加密库，性能和安全性有保障
4. **扩展方法**：编译时内联，无性能损失

这些通用方法和工具类将大大提高开发效率，减少重复代码，并提供统一的编程体验。
