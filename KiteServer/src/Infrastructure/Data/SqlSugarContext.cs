using SqlSugar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Models;

namespace Infrastructure.Data;

/// <summary>
/// SqlSugar 数据库上下文
/// </summary>
public class SqlSugarContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqlSugarContext> _logger;
    private readonly DatabaseSettings _databaseSettings;

    public SqlSugarContext(IConfiguration configuration, ILogger<SqlSugarContext> logger, IOptions<DatabaseSettings> databaseSettings)
    {
        _configuration = configuration;
        _logger = logger;
        _databaseSettings = databaseSettings.Value;
    }

    /// <summary>
    /// 创建 SqlSugar 客户端
    /// </summary>
    /// <returns></returns>
    public ISqlSugarClient CreateClient()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("数据库连接字符串未配置");
        }

        var config = new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = GetDbType(_databaseSettings.DatabaseType),
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            // CommandTimeOut = _databaseSettings.CommandTimeout, // 暂时注释掉，检查正确的属性名
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (c, p) =>
                {
                    // 自动添加审计字段
                    if (p.PropertyName == "CreateTime")
                    {
                        p.IsOnlyIgnoreInsert = true;
                    }
                    if (p.PropertyName == "UpdateTime")
                    {
                        p.IsOnlyIgnoreInsert = true;
                        p.IsOnlyIgnoreUpdate = false;
                    }
                    if (p.PropertyName == "IsDeleted")
                    {
                        p.IsIgnore = true;
                    }
                },
                EntityNameService = (type, entity) =>
                {
                    // 表名转换为小写下划线格式
                    entity.DbTableName = ToSnakeCase(entity.DbTableName);
                },
                // DataInfoCacheService = new HttpRuntimeCache() // 暂时注释掉，使用默认缓存
            },
            MoreSettings = new ConnMoreSettings
            {
                IsAutoRemoveDataCache = true,
                SqlServerCodeFirstNvarchar = true
            }
        };

        var client = new SqlSugarClient(config);

        // 配置日志
        client.Aop.OnLogExecuting = (sql, pars) =>
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("SQL执行: {Sql}", sql);
                if (pars != null && pars.Length > 0)
                {
                    _logger.LogDebug("参数: {Parameters}", string.Join(", ", pars.Select(p => $"{p.ParameterName}={p.Value}")));
                }
            }
        };

        client.Aop.OnError = (exp) =>
        {
            _logger.LogError(exp, "SQL执行错误: {Sql}", exp.Sql);
        };

        // 配置过滤器（软删除）- 暂时注释掉，后续实现
        // client.QueryFilter.Add(new TableFilterItem<object>
        // {
        //     FilterValue = filterDb =>
        //     {
        //         return new SqlFilterResult { Sql = " IsDeleted = 0 " };
        //     },
        //     IsJoinQuery = false
        // });

        return client;
    }

    /// <summary>
    /// 将字符串转换为下划线格式
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns></returns>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                if (i > 0)
                    result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// 根据配置获取 SqlSugar 数据库类型
    /// </summary>
    /// <param name="databaseType">数据库类型枚举</param>
    /// <returns>SqlSugar 数据库类型</returns>
    private static DbType GetDbType(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.MySQL => DbType.MySql,
            DatabaseType.PostgreSQL => DbType.PostgreSQL,
            DatabaseType.SQLServer => DbType.SqlServer,
            DatabaseType.SQLite => DbType.Sqlite,
            DatabaseType.Oracle => DbType.Oracle,
            _ => DbType.MySql
        };
    }
}
