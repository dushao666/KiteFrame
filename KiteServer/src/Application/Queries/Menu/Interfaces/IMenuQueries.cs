using Shared.Models.Menu;

namespace Application.Queries.Menu.Interfaces;

/// <summary>
/// 菜单查询接口
/// </summary>
public interface IMenuQueries
{
    /// <summary>
    /// 获取菜单列表（分页）
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <returns>菜单列表</returns>
    Task<ApiResult<PagedResult<MenuDto>>> GetMenusAsync(GetMenusRequest request);

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单信息</returns>
    Task<ApiResult<MenuDto>> GetMenuByIdAsync(long id);

    /// <summary>
    /// 获取菜单树形结构
    /// </summary>
    /// <returns>菜单树</returns>
    Task<ApiResult<List<MenuDto>>> GetMenuTreeAsync();

    /// <summary>
    /// 根据用户ID获取用户菜单权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户菜单列表</returns>
    Task<ApiResult<List<MenuDto>>> GetUserMenusAsync(long userId);
}
