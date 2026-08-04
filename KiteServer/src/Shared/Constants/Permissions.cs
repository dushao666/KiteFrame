namespace Shared.Constants;

/// <summary>
/// 权限点常量（与 sys_menu 种子数据中的 Permissions 权限标识一一对应）
/// 授权策略名即权限点名，端点通过 [Authorize(Policy = Permissions.Xxx)] 挂载
/// </summary>
public static class Permissions
{
    /// <summary>
    /// 首页查看
    /// </summary>
    public const string DashboardView = "dashboard:view";

    /// <summary>
    /// 用户管理 - 查看
    /// </summary>
    public const string SystemUserList = "system:user:list";

    /// <summary>
    /// 用户管理 - 新增
    /// </summary>
    public const string SystemUserAdd = "system:user:add";

    /// <summary>
    /// 用户管理 - 编辑
    /// </summary>
    public const string SystemUserEdit = "system:user:edit";

    /// <summary>
    /// 用户管理 - 删除
    /// </summary>
    public const string SystemUserDelete = "system:user:delete";

    /// <summary>
    /// 用户管理 - 重置密码
    /// </summary>
    public const string SystemUserReset = "system:user:reset";

    /// <summary>
    /// 用户管理 - 分配角色
    /// </summary>
    public const string SystemUserRole = "system:user:role";

    /// <summary>
    /// 角色管理 - 查看
    /// </summary>
    public const string SystemRoleList = "system:role:list";

    /// <summary>
    /// 角色管理 - 新增
    /// </summary>
    public const string SystemRoleAdd = "system:role:add";

    /// <summary>
    /// 角色管理 - 编辑
    /// </summary>
    public const string SystemRoleEdit = "system:role:edit";

    /// <summary>
    /// 角色管理 - 删除
    /// </summary>
    public const string SystemRoleDelete = "system:role:delete";

    /// <summary>
    /// 角色管理 - 分配权限
    /// </summary>
    public const string SystemRolePermission = "system:role:permission";

    /// <summary>
    /// 菜单管理 - 查看
    /// </summary>
    public const string SystemMenuList = "system:menu:list";

    /// <summary>
    /// 菜单管理 - 新增
    /// </summary>
    public const string SystemMenuAdd = "system:menu:add";

    /// <summary>
    /// 菜单管理 - 编辑
    /// </summary>
    public const string SystemMenuEdit = "system:menu:edit";

    /// <summary>
    /// 菜单管理 - 删除
    /// </summary>
    public const string SystemMenuDelete = "system:menu:delete";

    /// <summary>
    /// 系统监控 - 在线用户查看
    /// </summary>
    public const string MonitorOnlineList = "monitor:online:list";

    /// <summary>
    /// 系统监控 - 强制下线
    /// </summary>
    public const string MonitorOnlineLogout = "monitor:online:logout";

    /// <summary>
    /// 系统监控 - 登录日志查看
    /// </summary>
    public const string MonitorLoginLogList = "monitor:loginlog:list";

    /// <summary>
    /// 系统监控 - 登录日志删除
    /// </summary>
    public const string MonitorLoginLogDelete = "monitor:loginlog:delete";

    /// <summary>
    /// 系统监控 - 登录日志清空
    /// </summary>
    public const string MonitorLoginLogClear = "monitor:loginlog:clear";

    /// <summary>
    /// 系统监控 - 操作日志查看
    /// </summary>
    public const string MonitorOperLogList = "monitor:operlog:list";

    /// <summary>
    /// 系统监控 - 操作日志删除
    /// </summary>
    public const string MonitorOperLogDelete = "monitor:operlog:delete";

    /// <summary>
    /// 系统监控 - 操作日志清空
    /// </summary>
    public const string MonitorOperLogClear = "monitor:operlog:clear";

    /// <summary>
    /// 全部权限点（用于批量注册授权策略）
    /// </summary>
    public static readonly string[] All =
    {
        DashboardView,
        SystemUserList, SystemUserAdd, SystemUserEdit, SystemUserDelete, SystemUserReset, SystemUserRole,
        SystemRoleList, SystemRoleAdd, SystemRoleEdit, SystemRoleDelete, SystemRolePermission,
        SystemMenuList, SystemMenuAdd, SystemMenuEdit, SystemMenuDelete,
        MonitorOnlineList, MonitorOnlineLogout,
        MonitorLoginLogList, MonitorLoginLogDelete, MonitorLoginLogClear,
        MonitorOperLogList, MonitorOperLogDelete, MonitorOperLogClear
    };
}
