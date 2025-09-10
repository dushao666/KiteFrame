namespace Domain.Interfaces;

/// <summary>
/// 仓储接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : Domain.Entities.Base.BaseEntity
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns></returns>
    Task<T?> GetByIdAsync(long id);

    /// <summary>
    /// 根据条件获取单个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 获取所有实体
    /// </summary>
    /// <returns></returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <param name="predicate">查询条件</param>
    /// <param name="orderBy">排序</param>
    /// <returns></returns>
    Task<PagedResult<T>> GetPagedListAsync(
        int pageIndex, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null);

    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 获取记录数
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    Task<bool> InsertAsync(T entity);

    /// <summary>
    /// 批量插入实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    Task<bool> InsertRangeAsync(List<T> entities);

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    Task<bool> UpdateRangeAsync(List<T> entities);

    /// <summary>
    /// 根据条件更新
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="updateExpression">更新表达式</param>
    /// <returns></returns>
    Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> updateExpression);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(T entity);

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// 根据条件删除实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <param name="entities">实体列表</param>
    /// <returns></returns>
    Task<bool> DeleteRangeAsync(List<T> entities);

    /// <summary>
    /// 软删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="userId">操作用户ID</param>
    /// <returns></returns>
    Task<bool> SoftDeleteAsync(long id, long? userId = null);

    /// <summary>
    /// 根据条件软删除实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="userId">操作用户ID</param>
    /// <returns></returns>
    Task<bool> SoftDeleteAsync(Expression<Func<T, bool>> predicate, long? userId = null);
}
