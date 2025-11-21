using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Extensions;

/// <summary>
/// 服务集合扩展类
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加数据库服务
    /// </summary>
    public static void AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // 获取数据库配置
        var section = configuration.GetSection(DatabaseSettings.SectionName);
        var databaseTypeValue = section["DatabaseType"];
        var databaseType = string.IsNullOrEmpty(databaseTypeValue) 
            ? DatabaseType.MySQL 
            : Enum.Parse<DatabaseType>(databaseTypeValue);
        var enableSqlLog = bool.Parse(section["EnableSqlLog"] ?? "false");
        
        // 根据配置的数据库类型转换为SqlSugar的DbType和连接字符串键名
        var (dbType, connectionStringKey) = databaseType switch
        {
            DatabaseType.MySQL => (DbType.MySql, "MySQL"),
            DatabaseType.PostgreSQL => (DbType.PostgreSQL, "PostgreSQL"),
            DatabaseType.SQLServer => (DbType.SqlServer, "SQLServer"),
            DatabaseType.SQLite => (DbType.Sqlite, "SQLite"),
            DatabaseType.Oracle => (DbType.Oracle, "Oracle"),
            _ => (DbType.MySql, "MySQL") // 默认使用MySQL
        };

        // 获取对应数据库类型的连接字符串
        var connectionString = configuration.GetConnectionString(connectionStringKey) 
                              ?? configuration.GetConnectionString("DefaultConnection") 
                              ?? throw new InvalidOperationException($"未找到数据库类型 {databaseType} 对应的连接字符串配置");

        // SnowFlakeSingle.WorkId = 1;
        SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = dbType,
            ConnectionString = connectionString,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        },
        db =>
        {
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                //支持实体自定义属性：[Key]
                EntityService = (property, column) =>
                {
                    var attributes = property.GetCustomAttributes(true);
                    if (attributes.Any(it => it is KeyAttribute))
                    {
                        column.IsPrimarykey = true;
                    }
                },
                EntityNameService = (type, entity) =>
                {
                    var attributes = type.GetCustomAttributes(true);
                    if (attributes.Any(it => it is TableAttribute))
                    {
                        entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute)!.Name;
                    }
                }
            };
            
            //查询过滤器 - 自动过滤已删除的数据
            db.QueryFilter.AddTableFilter<IDeleted>(x => x.IsDeleted == false);
            
            //数据执行前事件
            db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                switch (entityInfo.OperationType)
                {
                    case DataFilterType.InsertByObject:
                        if (entityInfo.PropertyName == "CreateTime")
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                        if (entityInfo.PropertyName == "UpdateTime")
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                        if (entityInfo.PropertyName == "IsDeleted")
                        {
                            entityInfo.SetValue(false);
                        }
                        break;
                    case DataFilterType.UpdateByObject:
                        if (entityInfo.PropertyName == "UpdateTime")
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                        break;
                }
            };
            
            // 根据配置决定是否打印SQL
            if (enableSqlLog)
            {
                db.Aop.OnLogExecuting = (sql, pars) => 
                { 
                    Console.WriteLine($"SQL: {UtilMethods.GetNativeSql(sql, pars)}");
                };
            }
        });
        
        ISugarUnitOfWork<DBContext> context = new SugarUnitOfWork<DBContext>(sqlSugar);
        services.AddSingleton(context);
    }
}
