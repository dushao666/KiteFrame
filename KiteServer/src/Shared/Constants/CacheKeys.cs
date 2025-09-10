namespace Shared.Constants;

/// <summary>
/// 缓存键常量
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// 用户缓存键前缀
    /// </summary>
    public const string USER_PREFIX = "user:";

    /// <summary>
    /// 角色缓存键前缀
    /// </summary>
    public const string ROLE_PREFIX = "role:";

    /// <summary>
    /// 权限缓存键前缀
    /// </summary>
    public const string PERMISSION_PREFIX = "permission:";

    /// <summary>
    /// 菜单缓存键前缀
    /// </summary>
    public const string MENU_PREFIX = "menu:";

    /// <summary>
    /// 字典缓存键前缀
    /// </summary>
    public const string DICT_PREFIX = "dict:";

    /// <summary>
    /// 配置缓存键前缀
    /// </summary>
    public const string CONFIG_PREFIX = "config:";

    /// <summary>
    /// JWT Token 黑名单前缀
    /// </summary>
    public const string JWT_BLACKLIST_PREFIX = "jwt:blacklist:";

    /// <summary>
    /// 验证码缓存键前缀
    /// </summary>
    public const string CAPTCHA_PREFIX = "captcha:";

    /// <summary>
    /// 短信验证码缓存键前缀
    /// </summary>
    public const string SMS_CODE_PREFIX = "sms:code:";

    /// <summary>
    /// 邮箱验证码缓存键前缀
    /// </summary>
    public const string EMAIL_CODE_PREFIX = "email:code:";

    /// <summary>
    /// 在线用户缓存键前缀
    /// </summary>
    public const string ONLINE_USER_PREFIX = "online:user:";

    /// <summary>
    /// 获取用户缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public static string GetUserKey(long userId) => $"{USER_PREFIX}{userId}";

    /// <summary>
    /// 获取角色缓存键
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns></returns>
    public static string GetRoleKey(long roleId) => $"{ROLE_PREFIX}{roleId}";

    /// <summary>
    /// 获取用户权限缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public static string GetUserPermissionKey(long userId) => $"{PERMISSION_PREFIX}user:{userId}";

    /// <summary>
    /// 获取角色权限缓存键
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns></returns>
    public static string GetRolePermissionKey(long roleId) => $"{PERMISSION_PREFIX}role:{roleId}";

    /// <summary>
    /// 获取用户菜单缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public static string GetUserMenuKey(long userId) => $"{MENU_PREFIX}user:{userId}";

    /// <summary>
    /// 获取字典缓存键
    /// </summary>
    /// <param name="dictType">字典类型</param>
    /// <returns></returns>
    public static string GetDictKey(string dictType) => $"{DICT_PREFIX}{dictType}";

    /// <summary>
    /// 获取配置缓存键
    /// </summary>
    /// <param name="configKey">配置键</param>
    /// <returns></returns>
    public static string GetConfigKey(string configKey) => $"{CONFIG_PREFIX}{configKey}";

    /// <summary>
    /// 获取JWT黑名单缓存键
    /// </summary>
    /// <param name="jti">JWT ID</param>
    /// <returns></returns>
    public static string GetJwtBlacklistKey(string jti) => $"{JWT_BLACKLIST_PREFIX}{jti}";

    /// <summary>
    /// 获取验证码缓存键
    /// </summary>
    /// <param name="key">验证码键</param>
    /// <returns></returns>
    public static string GetCaptchaKey(string key) => $"{CAPTCHA_PREFIX}{key}";

    /// <summary>
    /// 获取短信验证码缓存键
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns></returns>
    public static string GetSmsCodeKey(string phone) => $"{SMS_CODE_PREFIX}{phone}";

    /// <summary>
    /// 获取邮箱验证码缓存键
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns></returns>
    public static string GetEmailCodeKey(string email) => $"{EMAIL_CODE_PREFIX}{email}";

    /// <summary>
    /// 获取在线用户缓存键
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    public static string GetOnlineUserKey(long userId) => $"{ONLINE_USER_PREFIX}{userId}";
}
