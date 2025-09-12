namespace Infrastructure.Extensions;

/// <summary>
/// 时间扩展方法
/// </summary>
public static class TimeExtensions
{
    /// <summary>
    /// 获取当前UTC时间
    /// </summary>
    /// <returns>UTC时间</returns>
    public static DateTime NewUTCTimeZone()
    {
        return DateTime.UtcNow;
    }

    /// <summary>
    /// 将时间戳转换为UTC时间
    /// </summary>
    /// <param name="ticks">时间戳</param>
    /// <returns>UTC时间</returns>
    public static DateTime ConvertToUTCTime(long ticks)
    {
        return new DateTime(new DateTime(1970, 1, 1, 0, 0, 0).Ticks + ticks * 10000);
    }

    /// <summary>
    /// 将时间戳转换为本地时间
    /// </summary>
    /// <param name="ticks">时间戳</param>
    /// <returns>本地时间</returns>
    public static DateTime ConvertToLocalTime(long ticks)
    {
        return new DateTime(new DateTime(1970, 1, 1, 0, 0, 0).Ticks + ticks * 10000).AddHours(8.0);
    }

    /// <summary>
    /// 获取当前时区时间（东八区）
    /// </summary>
    /// <returns>当前时区时间</returns>
    public static DateTime NewTimeZone()
    {
        return DateTime.UtcNow.AddHours(8.0);
    }

    /// <summary>
    /// 转换为UTC格式
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>UTC时间</returns>
    public static DateTime ToUtcFormat(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime();
    }

    /// <summary>
    /// 转换为DateOnly格式
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>DateOnly</returns>
    public static DateOnly? DateOnlyFormat(DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    /// <summary>
    /// 转换为TimeOnly格式
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>TimeOnly</returns>
    public static TimeOnly? TimeOnlyFormat(DateTime dateTime)
    {
        return TimeOnly.FromDateTime(dateTime);
    }

    /// <summary>
    /// 转换为DateOnly格式（可空）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>DateOnly</returns>
    public static DateOnly? DateOnlyFormat(DateTime? dateTime)
    {
        return dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null;
    }

    /// <summary>
    /// 转换为TimeOnly格式（可空）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>TimeOnly</returns>
    public static TimeOnly? TimeOnlyFormat(DateTime? dateTime)
    {
        return dateTime.HasValue ? TimeOnly.FromDateTime(dateTime.Value) : null;
    }

    /// <summary>
    /// 将DateOnly和TimeOnly组合为DateTime
    /// </summary>
    /// <param name="date">日期</param>
    /// <param name="time">时间</param>
    /// <returns>DateTime</returns>
    public static DateTime? DateTimeFormat(DateOnly? date, TimeOnly? time)
    {
        return date?.ToDateTime(time.GetValueOrDefault());
    }

    /// <summary>
    /// 获取当前日期
    /// </summary>
    /// <returns>当前日期</returns>
    public static DateOnly? CurrentDate()
    {
        return DateOnlyFormat(NewTimeZone());
    }

    /// <summary>
    /// 获取当前时间
    /// </summary>
    /// <returns>当前时间</returns>
    public static TimeOnly? CurrentTime()
    {
        return TimeOnlyFormat(NewTimeZone());
    }

    /// <summary>
    /// 将时间戳转换为DateTime
    /// </summary>
    /// <param name="timestamp">时间戳（秒）</param>
    /// <returns>DateTime</returns>
    public static DateTime ConvertTimestamp(double timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        if (timestamp > 9999999999.0)
        {
            throw new ArgumentException("Error timestamp, too large");
        }

        return dateTime.AddSeconds(timestamp).ToLocalTime();
    }

    /// <summary>
    /// 获取当前时间戳（秒）
    /// </summary>
    /// <returns>时间戳</returns>
    public static long GetCurrentTimestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000000;
    }

    /// <summary>
    /// 获取当前时间戳（毫秒）
    /// </summary>
    /// <returns>时间戳</returns>
    public static long GetCurrentMilliTimestamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
    }

    /// <summary>
    /// 获取时间戳（秒）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>时间戳</returns>
    public static long ToTimestamp(this DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks - 621355968000000000L) / 10000000;
    }

    /// <summary>
    /// 获取时间戳（毫秒）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>时间戳</returns>
    public static long ToMilliTimestamp(this DateTime dateTime)
    {
        return (dateTime.ToUniversalTime().Ticks - 621355968000000000L) / 10000;
    }

    /// <summary>
    /// 判断是否为今天
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>是否为今天</returns>
    public static bool IsToday(this DateTime dateTime)
    {
        return dateTime.Date == DateTime.Today;
    }

    /// <summary>
    /// 判断是否为本周
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>是否为本周</returns>
    public static bool IsThisWeek(this DateTime dateTime)
    {
        var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return dateTime.Date >= startOfWeek && dateTime.Date < endOfWeek;
    }

    /// <summary>
    /// 判断是否为本月
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>是否为本月</returns>
    public static bool IsThisMonth(this DateTime dateTime)
    {
        return dateTime.Year == DateTime.Now.Year && dateTime.Month == DateTime.Now.Month;
    }

    /// <summary>
    /// 判断是否为本年
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>是否为本年</returns>
    public static bool IsThisYear(this DateTime dateTime)
    {
        return dateTime.Year == DateTime.Now.Year;
    }

    /// <summary>
    /// 获取月初时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>月初时间</returns>
    public static DateTime StartOfMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// 获取月末时间
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>月末时间</returns>
    public static DateTime EndOfMonth(this DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// 获取周初时间（周一）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>周初时间</returns>
    public static DateTime StartOfWeek(this DateTime dateTime)
    {
        int diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// 获取周末时间（周日）
    /// </summary>
    /// <param name="dateTime">日期时间</param>
    /// <returns>周末时间</returns>
    public static DateTime EndOfWeek(this DateTime dateTime)
    {
        return dateTime.StartOfWeek().AddDays(6);
    }
}
