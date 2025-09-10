namespace Shared.Enums;

/// <summary>
/// 通用状态枚举
/// </summary>
public enum Status
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,
    
    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}

/// <summary>
/// 性别枚举
/// </summary>
public enum Gender
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// 男性
    /// </summary>
    Male = 1,
    
    /// <summary>
    /// 女性
    /// </summary>
    Female = 2
}

/// <summary>
/// 操作类型枚举
/// </summary>
public enum OperationType
{
    /// <summary>
    /// 查询
    /// </summary>
    Query = 0,
    
    /// <summary>
    /// 新增
    /// </summary>
    Create = 1,
    
    /// <summary>
    /// 更新
    /// </summary>
    Update = 2,
    
    /// <summary>
    /// 删除
    /// </summary>
    Delete = 3
}
