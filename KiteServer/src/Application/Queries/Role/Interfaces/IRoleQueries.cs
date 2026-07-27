using Shared.Models.Role;

namespace Application.Queries.Role.Interfaces;

/// <summary>
/// 角色查询服务接口
/// </summary>
public interface IRoleQueries
{
    /// <summary>
    /// 分页获取角色列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>角色列表</returns>
    Task<ApiResult<PagedResult<RoleDto>>> GetRolesAsync(GetRolesRequest request);

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns>角色信息</returns>
    Task<ApiResult<RoleDto>> GetRoleByIdAsync(long id);

    /// <summary>
    /// 获取所有启用的角色列表（用于下拉选择）
    /// </summary>
    /// <returns>角色列表</returns>
    Task<ApiResult<List<RoleDto>>> GetEnabledRolesAsync();

    /// <summary>
    /// 检查角色编码是否存在
    /// </summary>
    /// <param name="roleCode">角色编码</param>
    /// <param name="excludeId">排除的角色ID（用于更新时检查）</param>
    /// <returns>是否存在</returns>
    Task<ApiResult<bool>> CheckRoleCodeExistsAsync(string roleCode, long? excludeId = null);
}
