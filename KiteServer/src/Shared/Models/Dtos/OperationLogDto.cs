namespace Shared.Models.Dtos;

/// <summary>
/// 操作日志DTO
/// </summary>
public class OperationLogDto
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 模块名称
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 方法名称
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 操作类别（1：管理员，2：用户）
    /// </summary>
    public int OperatorType { get; set; }

    /// <summary>
    /// 操作类别描述
    /// </summary>
    public string OperatorTypeText => OperatorType == 1 ? "管理员" : "用户";

    /// <summary>
    /// 请求URL
    /// </summary>
    public string? OperUrl { get; set; }

    /// <summary>
    /// 主机地址
    /// </summary>
    public string? OperIp { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    public string? OperLocation { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? OperParam { get; set; }

    /// <summary>
    /// 返回参数
    /// </summary>
    public string? JsonResult { get; set; }

    /// <summary>
    /// 操作状态（1：正常，0：异常）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusText => Status == 1 ? "正常" : "异常";

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime OperTime { get; set; }

    /// <summary>
    /// 消耗时间（毫秒）
    /// </summary>
    public long? CostTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
} 