namespace Infrastructure.Data;

/// <summary>
/// 应用数据库上下文
/// </summary>
public class ApplicationDbContext
{
    private readonly ISqlSugarClient _sqlSugarClient;

    public ApplicationDbContext(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
    }

    /// <summary>
    /// SqlSugar 客户端
    /// </summary>
    public ISqlSugarClient Database => _sqlSugarClient;
}
