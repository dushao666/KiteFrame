namespace Shared.Models.Role;

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, ErrorMessage = "角色名称长度不能超过50个字符")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（唯一标识）
    /// </summary>
    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(100, ErrorMessage = "角色编码长度不能超过100个字符")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态（0：禁用，1：启用）
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 数据权限范围（1：全部，2：自定义，3：本部门，4：本部门及以下，5：仅本人）
    /// </summary>
    public int DataScope { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
