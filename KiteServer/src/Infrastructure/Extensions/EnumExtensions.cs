using System.ComponentModel;
using System.Reflection;

namespace Infrastructure.Extensions;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的整数值
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>整数值</returns>
    public static int GetValue(this Enum value)
    {
        return Convert.ToInt32(value);
    }

    /// <summary>
    /// 获取枚举的描述信息
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>描述信息</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) return value.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// 获取枚举的显示名称
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) return value.ToString();

        var attribute = field.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

    /// <summary>
    /// 判断枚举是否包含指定标志
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="flag">标志</param>
    /// <returns>是否包含</returns>
    public static bool HasFlag<T>(this T value, T flag) where T : Enum
    {
        return value.HasFlag(flag);
    }

    /// <summary>
    /// 获取枚举的所有值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns>所有枚举值</returns>
    public static IEnumerable<T> GetValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// 将字符串转换为枚举
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <returns>枚举值</returns>
    public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    /// <summary>
    /// 尝试将字符串转换为枚举
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="result">转换结果</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <returns>是否转换成功</returns>
    public static bool TryToEnum<T>(this string value, out T result, bool ignoreCase = true) where T : struct, Enum
    {
        return Enum.TryParse(value, ignoreCase, out result);
    }
}
