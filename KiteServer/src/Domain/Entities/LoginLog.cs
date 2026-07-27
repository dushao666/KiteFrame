namespace Domain.Entities;

/// <summary>
/// 登录日志实体
/// </summary>
[SugarTable("sys_login_log")]
public class LoginLog
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID", IsNullable = true)]
    public long? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "用户名", Length = 50, IsNullable = true)]
    public string? UserName { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "IP地址", Length = 50, IsNullable = true)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// IP归属地
    /// </summary>
    [SugarColumn(ColumnDescription = "IP归属地", Length = 200, IsNullable = true)]
    public string? IpLocation { get; set; }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器类型", Length = 50, IsNullable = true)]
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统", Length = 50, IsNullable = true)]
    public string? Os { get; set; }

    /// <summary>
    /// 登录状态（1：成功，0：失败）
    /// </summary>
    [SugarColumn(ColumnDescription = "登录状态", IsNullable = false)]
    public int Status { get; set; }

    /// <summary>
    /// 提示消息
    /// </summary>
    [SugarColumn(ColumnDescription = "提示消息", Length = 500, IsNullable = true)]
    public string? Message { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "登录时间", IsNullable = false)]
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;
} 