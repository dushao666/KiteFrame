namespace Repository.Extensions;

/// <summary>
/// 数据库连接解析结果
/// </summary>
/// <param name="DatabaseType">配置中的数据库类型</param>
/// <param name="DbType">SqlSugar 对应的数据库类型</param>
/// <param name="ConnectionString">连接字符串</param>
public sealed record DatabaseConnectionInfo(DatabaseType DatabaseType, DbType DbType, string ConnectionString);
