namespace Domain.Interfaces;

/// <summary>
/// 工作单元接口
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// SqlSugar 客户端
    /// </summary>
    ISqlSugarClient SqlSugarClient { get; }

    /// <summary>
    /// 获取仓储
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns></returns>
    IRepository<T> GetRepository<T>() where T : Domain.Entities.Base.BaseEntity, new();

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <returns></returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <returns></returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// 执行事务
    /// </summary>
    /// <param name="action">事务操作</param>
    /// <returns></returns>
    Task<bool> ExecuteTransactionAsync(Func<Task> action);

    /// <summary>
    /// 执行事务
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="func">事务操作</param>
    /// <returns></returns>
    Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> func);
}
