namespace Domain.Entities;

/// <summary>
/// 操作日志实体
/// </summary>
[SugarTable("sys_operation_log")]
public class OperationLog
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
    /// 模块名称
    /// </summary>
    [SugarColumn(ColumnDescription = "模块名称", Length = 50, IsNullable = true)]
    public string? Module { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    [SugarColumn(ColumnDescription = "业务类型", Length = 50, IsNullable = true)]
    public string? BusinessType { get; set; }

    /// <summary>
    /// 方法名称
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名称", Length = 200, IsNullable = true)]
    public string? Method { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方式", Length = 10, IsNullable = true)]
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 操作类别（1：管理员，2：用户）
    /// </summary>
    [SugarColumn(ColumnDescription = "操作类别")]
    public int OperatorType { get; set; } = 1;

    /// <summary>
    /// 请求URL
    /// </summary>
    [SugarColumn(ColumnDescription = "请求URL", Length = 500, IsNullable = true)]
    public string? OperUrl { get; set; }

    /// <summary>
    /// 主机地址
    /// </summary>
    [SugarColumn(ColumnDescription = "主机地址", Length = 50, IsNullable = true)]
    public string? OperIp { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    [SugarColumn(ColumnDescription = "操作地点", Length = 200, IsNullable = true)]
    public string? OperLocation { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDescription = "请求参数", ColumnDataType = "text", IsNullable = true)]
    public string? OperParam { get; set; }

    /// <summary>
    /// 返回参数
    /// </summary>
    [SugarColumn(ColumnDescription = "返回参数", ColumnDataType = "text", IsNullable = true)]
    public string? JsonResult { get; set; }

    /// <summary>
    /// 操作状态（1：正常，0：异常）
    /// </summary>
    [SugarColumn(ColumnDescription = "操作状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 错误消息
    /// </summary>
    [SugarColumn(ColumnDescription = "错误消息", ColumnDataType = "text", IsNullable = true)]
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    [SugarColumn(ColumnDescription = "操作时间", IsNullable = false)]
    public DateTime OperTime { get; set; }

    /// <summary>
    /// 消耗时间（毫秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "消耗时间", IsNullable = true)]
    public long? CostTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = false)]
    public DateTime CreateTime { get; set; } = DateTime.Now;
} 