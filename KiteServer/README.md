# KiteServer - .NET 9 后台管理系统

基于 .NET 9 + SqlSugar + MySQL 的现代化后台管理系统 API。

## 🏗️ 技术架构

### 核心技术栈
- **.NET 9** - 最新的 .NET 框架
- **SqlSugar** - 高性能 ORM 框架
- **MySQL** - 主数据库
- **DbUp** - 数据库迁移（Flyway 式 SQL 脚本，启动时自动执行）
- **Redis** - 缓存和会话存储
- **Serilog** - 结构化日志
- **JWT** - 身份认证
- **Mapster** - 对象映射
- **FluentValidation** - 数据验证
- **MediatR** - CQRS 模式

### 架构模式
- **洋葱架构** - 清晰的分层结构
- **CQRS** - 命令查询职责分离
- **Repository + UnitOfWork** - 数据访问层抽象
- **DDD** - 领域驱动设计

## 📁 项目结构

```
KiteServer/
├── src/                        # 源代码
│   ├── Api/                    # Web API 层
│   ├── Application/            # 应用服务层
│   ├── Domain/                 # 领域层
│   ├── Infrastructure/         # 基础设施层
│   ├── Repository/             # 仓储层（DBContext、迁移执行器、Sql/Migrations 迁移脚本，按数据库方言分子目录）
│   └── Shared/                 # 共享层
├── tests/                      # 测试项目
│   ├── UnitTests/              # 单元测试
│   └── IntegrationTests/       # 集成测试
├── docs/                       # 文档
├── .env.example               # 环境变量示例
└── README.md                  # 项目说明
```

## 🚀 快速开始

### 环境要求
- .NET 9 SDK
- MySQL 8.0+
- Redis (可选)

### 安装步骤

1. **克隆项目**
   ```bash
   git clone <repository-url>
   cd KiteServer
   ```

2. **配置环境变量**
   ```bash
   cp .env.example .env
   # 编辑 .env 文件，修改数据库连接等配置
   ```

3. **配置数据库**
   - 修改 `appsettings.Development.json` 中的连接字符串（`ConnectionStrings:DefaultConnection`）
   - 无需手动建表：首次启动时 DbUp 自动创建表结构与基础数据，并在版本表 `sys_schema_versions` 中记录已执行的迁移脚本

4. **运行项目**
   ```bash
   dotnet run --project src/Api
   ```

5. **访问 API 文档**
   - 开发环境：http://localhost:5153/openapi

## ⚙️ 配置说明

### 配置文件
- `appsettings.json` - 基础配置
- `appsettings.Development.json` - 开发环境配置
- `appsettings.Production.json` - 生产环境配置

### 主要配置项

#### 数据库连接
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=KiteServer_Dev;Uid=root;Pwd=123456;CharSet=utf8mb4;",
    "Redis": "localhost:6379"
  }
}
```

#### JWT 配置
```json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKey",
    "Issuer": "KiteServer",
    "Audience": "KiteServer",
    "ExpiryMinutes": 60
  }
}
```

#### Serilog 日志配置
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

#### 数据库迁移（DbUp）

- 迁移脚本位于 `src/Repository/Sql/Migrations/{方言}/`（Flyway 风格命名 `V00xx__描述.sql`，按数据库方言分子目录存放，当前仅维护 `MySql/`），嵌入 Repository 程序集，应用启动时自动执行当前数据库方言目录下尚未执行的脚本。
- 表结构变更请新增下一个编号的迁移脚本，不要修改已执行的脚本；完整约定见 `docs/项目开发规范.md` 第 10 章。

## 📝 日志功能

### Serilog 特性
- **结构化日志** - JSON 格式输出
- **多种输出** - 控制台 + 文件
- **日志级别** - Debug/Information/Warning/Error
- **日志轮转** - 按天分割，自动清理
- **性能监控** - 请求响应时间记录
- **异常追踪** - 详细的异常信息

### 日志扩展方法
```csharp
// 操作日志
logger.LogOperationStart("用户登录", new { UserId = 123 });
logger.LogOperationSuccess("用户登录", result, elapsedMs);
logger.LogOperationError("用户登录", exception, parameters);

// 业务日志
logger.LogBusinessWarning("用户权限不足: {UserId}", userId);
logger.LogSecurity("检测到异常登录: {IP}", ipAddress);
logger.LogPerformance("数据库查询", elapsedMs, threshold: 1000);
```

## 🔧 开发指南

### 编译项目
```bash
dotnet build
```

### 运行测试
```bash
dotnet test
```

### 代码规范
- 使用 .NET 9 新语法特性
- 遵循 Clean Architecture 原则
- 所有异常必须记录日志
- 使用 try-catch 包装业务逻辑
- 接口和实现类使用统一参数对象

## 📊 监控和运维

### 健康检查
- 数据库连接检查
- Redis 连接检查
- 应用程序状态检查

### 性能监控
- 请求响应时间
- 数据库查询性能
- 内存使用情况
- 异常统计

## 🔒 安全特性

### 身份认证
- JWT Token 认证
- Refresh Token 机制
- 密码加密存储

### 权限控制
- 基于角色的访问控制 (RBAC)
- 接口级权限验证
- 数据级权限过滤

### 安全防护
- CORS 跨域配置
- 请求频率限制
- SQL 注入防护
- XSS 攻击防护

## 📈 后续计划

- [ ] 实现领域层业务实体
- [ ] 创建应用服务层
- [ ] 开发 API 控制器
- [ ] 集成身份认证
- [ ] 添加权限管理
- [ ] 实现文件上传
- [ ] 添加缓存策略
- [ ] 完善单元测试
- [ ] 部署配置优化

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支
3. 提交代码变更
4. 推送到分支
5. 创建 Pull Request

## 📄 许可证

MIT License
