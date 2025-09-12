namespace Shared.Models.User;

/// <summary>
/// 用户统计信息DTO
/// </summary>
public class UserStatisticsDto
{
    /// <summary>
    /// 总用户数
    /// </summary>
    public int TotalUsers { get; set; }
    
    /// <summary>
    /// 活跃用户数
    /// </summary>
    public int ActiveUsers { get; set; }
    
    /// <summary>
    /// 非活跃用户数
    /// </summary>
    public int InactiveUsers { get; set; }
    
    /// <summary>
    /// 今日注册用户数
    /// </summary>
    public int TodayRegistrations { get; set; }
}
