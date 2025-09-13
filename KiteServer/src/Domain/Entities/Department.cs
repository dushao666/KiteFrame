namespace Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
[SugarTable("sys_department")]
public class Department : BaseEntity
{
    /// <summary>
    /// 父部门ID（0表示根部门）
    /// </summary>
    [SugarColumn(ColumnDescription = "父部门ID")]
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", Length = 50, IsNullable = false)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码（唯一标识）
    /// </summary>
    [SugarColumn(ColumnDescription = "部门编码", Length = 100, IsNullable = false)]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>
    /// 部门类型（1：公司，2：部门，3：小组）
    /// </summary>
    [SugarColumn(ColumnDescription = "部门类型")]
    public DepartmentType DepartmentType { get; set; } = DepartmentType.Department;

    /// <summary>
    /// 负责人用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "负责人用户ID", IsNullable = true)]
    public long? LeaderId { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 子部门列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Department> Children { get; set; } = new List<Department>();

    /// <summary>
    /// 父部门（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public Department? Parent { get; set; }

    /// <summary>
    /// 负责人（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public User? Leader { get; set; }

    /// <summary>
    /// 部门下的用户列表（导航属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<User> Users { get; set; } = new List<User>();
}

/// <summary>
/// 部门类型枚举
/// </summary>
public enum DepartmentType
{
    /// <summary>
    /// 公司
    /// </summary>
    Company = 1,

    /// <summary>
    /// 部门
    /// </summary>
    Department = 2,

    /// <summary>
    /// 小组
    /// </summary>
    Group = 3
}
