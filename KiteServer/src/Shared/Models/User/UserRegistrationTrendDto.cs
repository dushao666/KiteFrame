namespace Shared.Models.User;

/// <summary>
/// 用户注册趋势DTO
/// </summary>
public class UserRegistrationTrendDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// 注册数量
    /// </summary>
    public int Count { get; set; }
}
