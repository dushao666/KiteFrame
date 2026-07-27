namespace Shared.Models.Dtos;

/// <summary>
/// 部门DTO
/// </summary>
public class DepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>
    /// 部门类型（1：公司，2：部门，3：小组）
    /// </summary>
    public int DepartmentType { get; set; }

    /// <summary>
    /// 负责人用户ID
    /// </summary>
    public long? LeaderId { get; set; }

    /// <summary>
    /// 负责人姓名
    /// </summary>
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 子部门列表
    /// </summary>
    public List<DepartmentDto> Children { get; set; } = new List<DepartmentDto>();
}
