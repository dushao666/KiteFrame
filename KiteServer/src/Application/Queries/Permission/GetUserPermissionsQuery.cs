namespace Application.Queries.Permission;

/// <summary>
/// 获取用户权限查询
/// </summary>
public record GetUserPermissionsQuery(long UserId) : IRequest<UserPermissionDto>;
