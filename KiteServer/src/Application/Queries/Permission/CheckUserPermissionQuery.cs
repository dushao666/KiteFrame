namespace Application.Queries.Permission;

/// <summary>
/// 检查用户权限查询
/// </summary>
public record CheckUserPermissionQuery(long UserId, string Permission) : IRequest<bool>;
