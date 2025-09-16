namespace Shared.Models.User;

/// <summary>
/// 分配用户角色请求
/// </summary>
public class AssignUserRolesRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    [Required(ErrorMessage = "角色列表不能为空")]
    public List<long> RoleIds { get; set; } = new();
} 