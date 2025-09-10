namespace Shared.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 判断字符串是否为空或空白
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>是否为空或空白</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 判断字符串是否不为空且不为空白
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <returns>是否不为空且不为空白</returns>
    public static bool IsNotNullOrWhiteSpace(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// 移除字符串末尾的指定后缀
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <param name="suffix">要移除的后缀</param>
    /// <returns>移除后缀后的字符串</returns>
    public static string RemovePostFix(this string value, string suffix)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(suffix))
            return value;

        if (value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            return value[..^suffix.Length];

        return value;
    }

    /// <summary>
    /// 移除字符串开头的指定前缀
    /// </summary>
    /// <param name="value">原字符串</param>
    /// <param name="prefix">要移除的前缀</param>
    /// <returns>移除前缀后的字符串</returns>
    public static string RemovePreFix(this string value, string prefix)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(prefix))
            return value;

        if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return value[prefix.Length..];

        return value;
    }
}
