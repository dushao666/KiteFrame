namespace Infrastructure.Data;

/// <summary>
/// 仓储实现
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class Repository<T> : IRepository<T> where T : Domain.Entities.Base.BaseEntity, new()
{
    private readonly ISqlSugarClient _sqlSugarClient;

    public Repository(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns></returns>
    public async Task<T?> GetByIdAsync(long id)
    {
        return await _sqlSugarClient.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 根据条件获取单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _sqlSugarClient.Queryable<T>().FirstAsync(predicate);
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> GetAllAsync()
    {
        return await _sqlSugarClient.Queryable<T>().ToListAsync();
    }

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _sqlSugarClient.Queryable<T>().Where(predicate).ToListAsync();
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <param name="predicate">查询条件</param>
    /// <param name="orderBy">排序</param>
    /// <returns></returns>
    public async Task<PagedResult<T>> GetPagedListAsync(
        int pageIndex, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null)
    {
        var query = _sqlSugarClient.Queryable<T>();
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        RefAsync<int> totalCount = 0;
        var items = await query.ToPageListAsync(pageIndex, pageSize, totalCount);

        return new PagedResult<T>(items, totalCount, pageIndex, pageSize);
    }

    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _sqlSugarClient.Queryable<T>().AnyAsync(predicate);
    }

    /// <summary>
    /// 获取记录数
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _sqlSugarClient.Queryable<T>();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return await query.CountAsync();
    }

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    public async Task<bool> InsertAsync(T entity)
    {
        entity.CreateTime = DateTime.Now;
        var result = await _sqlSugarClient.Insertable(entity).ExecuteReturnIdentityAsync();
        entity.Id = result;
        return result > 0;
    }

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    public async Task<bool> InsertRangeAsync(List<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.CreateTime = DateTime.Now;
        }
        var result = await _sqlSugarClient.Insertable(entities).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(T entity)
    {
        entity.UpdateTime = DateTime.Now;
        var result = await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    public async Task<bool> UpdateRangeAsync(List<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdateTime = DateTime.Now;
        }
        var result = await _sqlSugarClient.Updateable(entities).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 根据条件更新
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="updateExpression">更新表达式</param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression)
    {
        var result = await _sqlSugarClient.Updateable<T>()
            .SetColumns(updateExpression)
            .Where(predicate)
            .ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(T entity)
    {
        var result = await _sqlSugarClient.Deleteable(entity).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var result = await _sqlSugarClient.Deleteable<T>().In(id).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 根据条件删除实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await _sqlSugarClient.Deleteable<T>().Where(predicate).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    public async Task<bool> DeleteRangeAsync(List<T> entities)
    {
        var result = await _sqlSugarClient.Deleteable(entities).ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 软删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="userId">操作用户ID</param>
    /// <returns></returns>
    public async Task<bool> SoftDeleteAsync(long id, long? userId = null)
    {
        var result = await _sqlSugarClient.Updateable<T>()
            .SetColumns(x => new T 
            { 
                IsDeleted = true, 
                DeleteTime = DateTime.Now,
                DeleteUserId = userId
            })
            .Where(x => x.Id == id)
            .ExecuteCommandAsync();
        return result > 0;
    }

    /// <summary>
    /// 根据条件软删除实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="userId">操作用户ID</param>
    /// <returns></returns>
    public async Task<bool> SoftDeleteAsync(Expression<Func<T, bool>> predicate, long? userId = null)
    {
        var result = await _sqlSugarClient.Updateable<T>()
            .SetColumns(x => new T 
            { 
                IsDeleted = true, 
                DeleteTime = DateTime.Now,
                DeleteUserId = userId
            })
            .Where(predicate)
            .ExecuteCommandAsync();
        return result > 0;
    }
}
