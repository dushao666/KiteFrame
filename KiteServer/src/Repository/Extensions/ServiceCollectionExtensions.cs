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
        // SnowFlakeSingle.WorkId = 1;
        SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
        {
            DbType = DbType.MySql,
            ConnectionString = configuration.GetConnectionString("DefaultConnection"),
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
                    case DataFilterType.DeleteByObject:
                        if (entityInfo.PropertyName == "UpdateTime")
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                        if (entityInfo.PropertyName == "DeleteTime")
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                        if (entityInfo.PropertyName == "IsDeleted")
                        {
                            entityInfo.SetValue(true);
                        }
                        break;
                }
            };
            
            // 开发环境下打印SQL
            #if DEBUG
            db.Aop.OnLogExecuting = (sql, pars) => 
            { 
                Console.WriteLine($"SQL: {UtilMethods.GetNativeSql(sql, pars)}");
            };
            #endif
        });
        
        ISugarUnitOfWork<DBContext> context = new SugarUnitOfWork<DBContext>(sqlSugar);
        services.AddSingleton(context);
    }
}
