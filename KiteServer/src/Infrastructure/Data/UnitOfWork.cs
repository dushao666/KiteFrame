namespace Infrastructure.Data;

/// <summary>
/// 工作单元实现
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed = false;

    public UnitOfWork(ISqlSugarClient sqlSugarClient, ILogger<UnitOfWork> logger)
    {
        _sqlSugarClient = sqlSugarClient;
        _logger = logger;
        _repositories = new Dictionary<Type, object>();
    }

    /// <summary>
    /// SqlSugar 客户端
    /// </summary>
    public ISqlSugarClient SqlSugarClient => _sqlSugarClient;

    /// <summary>
    /// 获取仓储
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <returns></returns>
    public IRepository<T> GetRepository<T>() where T : Domain.Entities.Base.BaseEntity, new()
    {
        var type = typeof(T);
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<T>(_sqlSugarClient);
        }
        return (IRepository<T>)_repositories[type];
    }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns></returns>
    public async Task BeginTransactionAsync()
    {
        try
        {
            _sqlSugarClient.Ado.BeginTran();
            _logger.LogDebug("事务已开始");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "开始事务失败");
            throw;
        }
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <returns></returns>
    public async Task CommitTransactionAsync()
    {
        try
        {
            _sqlSugarClient.Ado.CommitTran();
            _logger.LogDebug("事务已提交");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "提交事务失败");
            await RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <returns></returns>
    public async Task RollbackTransactionAsync()
    {
        try
        {
            _sqlSugarClient.Ado.RollbackTran();
            _logger.LogDebug("事务已回滚");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回滚事务失败");
            throw;
        }
    }

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns></returns>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            // SqlSugar 不需要显式保存更改，操作会立即执行
            await Task.CompletedTask;
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存更改失败");
            throw;
        }
    }

    /// <summary>
    /// 执行事务
    /// </summary>
    /// <param name="action">事务操作</param>
    /// <returns></returns>
    public async Task<bool> ExecuteTransactionAsync(Func<Task> action)
    {
        try
        {
            await BeginTransactionAsync();
            await action();
            await CommitTransactionAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行事务失败");
            await RollbackTransactionAsync();
            return false;
        }
    }

    /// <summary>
    /// 执行事务
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    /// <param name="func">事务操作</param>
    /// <returns></returns>
    public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> func)
    {
        try
        {
            await BeginTransactionAsync();
            var result = await func();
            await CommitTransactionAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行事务失败");
            await RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                _sqlSugarClient?.Dispose();
                _repositories.Clear();
                _logger.LogDebug("UnitOfWork 已释放");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "释放 UnitOfWork 失败");
            }
            _disposed = true;
        }
    }
}
