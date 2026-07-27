namespace Application.Commands.Role;

/// <summary>
/// 更新角色命令
/// </summary>
public class UpdateRoleCommand : IRequest<ApiResult<bool>>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码（唯一标识）
    /// </summary>
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
    /// 数据权限范围
    /// </summary>
    public DataScope DataScope { get; set; } = DataScope.All;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
