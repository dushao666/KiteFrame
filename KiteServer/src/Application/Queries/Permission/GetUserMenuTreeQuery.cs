namespace Application.Queries.Permission;

/// <summary>
/// 获取用户菜单树查询
/// </summary>
public record GetUserMenuTreeQuery(long UserId) : IRequest<List<MenuDto>>;
