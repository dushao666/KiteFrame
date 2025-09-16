namespace Domain.Entities;

/// <summary>
/// 在线用户实体
/// </summary>
[SugarTable("sys_online_user")]
public class OnlineUser
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "主键ID")]
    public long Id { get; set; }

    /// <summary>
    /// 用户会话ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户会话ID", Length = 200, IsNullable = false)]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID", IsNullable = false)]
    public long UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "用户名", Length = 50, IsNullable = false)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 50, IsNullable = true)]
    public string? RealName { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(ColumnDescription = "部门ID", IsNullable = true)]
    public long? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", Length = 50, IsNullable = true)]
    public string? DeptName { get; set; }

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
    /// 登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "登录时间", IsNullable = false)]
    public DateTime LoginTime { get; set; }

    /// <summary>
    /// 最后访问时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后访问时间", IsNullable = false)]
    public DateTime LastAccessTime { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(ColumnDescription = "过期时间", IsNullable = false)]
    public DateTime ExpireTime { get; set; }

    /// <summary>
    /// 状态（1：在线，0：离线）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsNullable = false)]
    public DateTime UpdateTime { get; set; } = DateTime.Now;
} 