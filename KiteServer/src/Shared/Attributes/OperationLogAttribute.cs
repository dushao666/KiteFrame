namespace Shared.Attributes;

/// <summary>
/// 操作日志特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class OperationLogAttribute : Attribute
{
    /// <summary>
    /// 模块名称
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string? BusinessType { get; set; }

    /// <summary>
    /// 是否记录请求参数
    /// </summary>
    public bool LogParams { get; set; } = true;

    /// <summary>
    /// 是否记录返回结果
    /// </summary>
    public bool LogResult { get; set; } = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    public OperationLogAttribute()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="module">模块名称</param>
    /// <param name="businessType">业务类型</param>
    public OperationLogAttribute(string module, string businessType)
    {
        Module = module;
        BusinessType = businessType;
    }
}
