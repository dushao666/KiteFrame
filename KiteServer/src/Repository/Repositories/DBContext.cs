using Domain.Entities;
using Domain.Interfaces;

namespace Repository;

/// <summary>
/// 数据库上下文
/// </summary>
public class DBContext : SugarUnitOfWork
{
    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;
}

/// <summary>
/// 数据集合
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class DbSet<T> : SimpleClient<T> where T : class, IDeleted, new()
{
    // 移除了逻辑删除的重写方法，因为已经在ServiceCollectionExtensions中通过AOP配置了逻辑删除
    // 这样让DBContext更加优雅，职责更加单一
}
