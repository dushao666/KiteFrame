# KiteServer 数据库脚本说明

## 目录结构
```
Sql/
├── README.md                   # 说明文档
├── 01_CreateTables.sql         # 建表脚本
└── 02_InitData.sql            # 初始化数据脚本（可选）
```

## 执行顺序

### 1. 创建数据库和表结构
执行 `01_CreateTables.sql` 脚本，该脚本会：
- 创建数据库 `kite_prod`（生产环境）
- 创建数据库 `kite_test`（开发/测试环境）
- 创建数据库 `kite_log`（日志数据库）
- 创建用户表 `sys_user`
- 插入默认管理员账户

### 2. 默认管理员账户
- 用户名：`admin`
- 密码：`123456`
- 邮箱：`admin@kiteserver.com`

## 数据库连接配置

### 开发/测试环境
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=kite_test;Uid=root;Pwd=123456;CharSet=utf8mb4;"
}
```

### 生产环境
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=kite_prod;Uid=root;Pwd=123456;CharSet=utf8mb4;"
}
```

## 表结构说明

### sys_user (用户表)
| 字段名 | 类型 | 长度 | 允许空 | 默认值 | 说明 |
|--------|------|------|--------|--------|------|
| Id | bigint | - | NO | AUTO_INCREMENT | 主键ID |
| UserName | varchar | 50 | NO | - | 用户名（唯一） |
| Password | varchar | 100 | NO | - | 密码（加密存储） |
| Email | varchar | 100 | YES | NULL | 邮箱 |
| Phone | varchar | 20 | YES | NULL | 手机号 |
| RealName | varchar | 50 | YES | NULL | 真实姓名 |
| Avatar | varchar | 200 | YES | NULL | 头像URL |
| Status | int | - | NO | 1 | 状态（0：禁用，1：启用） |
| LastLoginTime | datetime | - | YES | NULL | 最后登录时间 |
| LastLoginIp | varchar | 50 | YES | NULL | 最后登录IP |
| Remark | varchar | 500 | YES | NULL | 备注 |
| CreateTime | datetime | - | NO | CURRENT_TIMESTAMP | 创建时间 |
| UpdateTime | datetime | - | NO | CURRENT_TIMESTAMP | 更新时间 |
| CreateUserId | bigint | - | YES | NULL | 创建用户ID |
| UpdateUserId | bigint | - | YES | NULL | 更新用户ID |
| IsDeleted | tinyint(1) | - | NO | 0 | 是否删除（逻辑删除） |
| DeleteTime | datetime | - | YES | NULL | 删除时间 |
| DeleteUserId | bigint | - | YES | NULL | 删除用户ID |

### 索引说明
- `PRIMARY KEY`: Id（主键）
- `UNIQUE KEY`: UserName（用户名唯一索引）
- `KEY`: Email, Phone, Status, IsDeleted, CreateTime（普通索引）

## 注意事项

1. **字符集**：所有表都使用 `utf8mb4` 字符集，支持完整的UTF-8字符
2. **逻辑删除**：系统使用逻辑删除，通过 `IsDeleted` 字段标记
3. **时间戳**：`CreateTime` 和 `UpdateTime` 会自动维护
4. **密码安全**：密码使用SHA512加密存储，提供更高的安全性
5. **索引优化**：为常用查询字段添加了索引以提高查询性能

## 扩展说明

如需添加新表，请：
1. 在 `Domain/Entities` 中定义实体类
2. 继承 `BaseEntity` 基类
3. 使用 `SugarTable` 和 `SugarColumn` 特性标注
4. 在此目录下创建对应的SQL脚本
5. 更新此README文档
