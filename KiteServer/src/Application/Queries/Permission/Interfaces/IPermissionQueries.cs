using Shared.Models.Permission;

namespace Application.Queries.Permission.Interfaces;

/// <summary>
/// 权限查询服务接口
/// </summary>
public interface IPermissionQueries
{
    /// <summary>
    /// 获取用户权限信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户权限信息</returns>
    Task<ApiResult<UserPermissionDto>> GetUserPermissionsAsync(long userId);

    /// <summary>
    /// 获取用户菜单树
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>菜单树</returns>
    Task<ApiResult<List<MenuDto>>> GetUserMenuTreeAsync(long userId);

    /// <summary>
    /// 检查用户权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permission">权限标识</param>
    /// <returns>是否有权限</returns>
    Task<ApiResult<bool>> CheckUserPermissionAsync(long userId, string permission);

    /// <summary>
    /// 获取所有菜单列表（用于权限配置）
    /// </summary>
    /// <returns>菜单列表</returns>
    Task<ApiResult<List<MenuDto>>> GetAllMenusAsync();

    /// <summary>
    /// 根据角色ID获取菜单权限
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>菜单权限列表</returns>
    Task<ApiResult<List<long>>> GetRoleMenuIdsAsync(long roleId);
} 