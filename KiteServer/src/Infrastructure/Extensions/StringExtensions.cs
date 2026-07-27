namespace Infrastructure.Extensions;

/// <summary>
/// 字符串扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 移除末尾指定字符串
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <param name="postFixes">要移除的字符串</param>
    /// <returns>移除后的字符串</returns>
    public static string RemovePostFix(this string str, params string[] postFixes)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        if (postFixes.Length == 0) return str;
        
        foreach (var postFix in postFixes)
        {
            if (str.EndsWith(postFix)) 
                return str.Substring(0, str.Length - postFix.Length);
        }
        return str;
    }

    /// <summary>
    /// 移除开头指定字符串
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <param name="preFixes">要移除的字符串</param>
    /// <returns>移除后的字符串</returns>
    public static string RemovePreFix(this string str, params string[] preFixes)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        if (preFixes.Length == 0) return str;
        
        foreach (var preFix in preFixes)
        {
            if (str.StartsWith(preFix)) 
                return str.Substring(preFix.Length);
        }
        return str;
    }

    /// <summary>
    /// 判断字符串是否为空或空白字符
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>是否为空</returns>
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 判断字符串是否不为空且不为空白字符
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>是否不为空</returns>
    public static bool IsNotNullOrWhiteSpace(this string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 安全截取字符串
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <param name="maxLength">最大长度</param>
    /// <param name="suffix">超长时的后缀</param>
    /// <returns>截取后的字符串</returns>
    public static string Truncate(this string str, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
            return str;

        return str.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// 转换为驼峰命名
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <returns>驼峰命名字符串</returns>
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length < 2)
            return str;

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// 转换为帕斯卡命名
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <returns>帕斯卡命名字符串</returns>
    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length < 2)
            return str;

        return char.ToUpperInvariant(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// 隐藏敏感信息
    /// </summary>
    /// <param name="info">敏感信息</param>
    /// <param name="left">左侧保留字符数</param>
    /// <param name="right">右侧保留字符数</param>
    /// <param name="basedOnLeft">当字符串长度不足时，是否基于左侧保留</param>
    /// <returns>隐藏后的字符串</returns>
    public static string HideSensitiveInfo(this string info, int left, int right, bool basedOnLeft = true)
    {
        if (string.IsNullOrEmpty(info))
            return "";

        var sb = new System.Text.StringBuilder();
        int hideLength = info.Length - left - right;
        
        if (hideLength > 0)
        {
            string leftPart = info.Substring(0, left);
            string rightPart = info.Substring(info.Length - right);
            sb.Append(leftPart);
            
            for (int i = 0; i < hideLength; i++)
            {
                sb.Append("*");
            }
            
            sb.Append(rightPart);
        }
        else if (basedOnLeft)
        {
            if (info.Length > left && left > 0)
            {
                sb.Append(info.Substring(0, left) + "****");
            }
            else
            {
                sb.Append(info.Substring(0, 1) + "****");
            }
        }
        else
        {
            if (info.Length > right && right > 0)
            {
                sb.Append("****" + info.Substring(info.Length - right));
            }
            else
            {
                sb.Append("****" + info.Substring(info.Length - 1));
            }
        }

        return sb.ToString();
    }
}
