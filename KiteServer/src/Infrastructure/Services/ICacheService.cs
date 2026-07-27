namespace Infrastructure.Services;

/// <summary>
/// 缓存服务接口
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>缓存值</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">缓存键</param>
    /// <param name="value">缓存值</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除缓存
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除缓存
    /// </summary>
    /// <param name="keys">缓存键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="expiration">过期时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取缓存剩余过期时间
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据模式删除缓存
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有匹配模式的键
    /// </summary>
    /// <param name="pattern">匹配模式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hash操作：设置Hash字段值
    /// </summary>
    /// <param name="key">Hash键</param>
    /// <param name="field">字段名</param>
    /// <param name="value">字段值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task HashSetAsync(string key, string field, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hash操作：获取Hash字段值
    /// </summary>
    /// <param name="key">Hash键</param>
    /// <param name="field">字段名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<string?> HashGetAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hash操作：删除Hash字段
    /// </summary>
    /// <param name="key">Hash键</param>
    /// <param name="field">字段名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<bool> HashDeleteAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hash操作：获取Hash所有字段和值
    /// </summary>
    /// <param name="key">Hash键</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<Dictionary<string, string>> HashGetAllAsync(string key, CancellationToken cancellationToken = default);
}
