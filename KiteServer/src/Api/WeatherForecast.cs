namespace Api;

/// <summary>
/// 天气预报
/// </summary>
[SwaggerSchema(Description = "天气预报信息")]
public class WeatherForecast
{
    /// <summary>
    /// 日期
    /// </summary>
    [SwaggerSchema(Description = "预报日期")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// 摄氏温度
    /// </summary>
    [SwaggerSchema(Description = "摄氏温度")]
    public int TemperatureC { get; set; }

    /// <summary>
    /// 华氏温度
    /// </summary>
    [SwaggerSchema(Description = "华氏温度")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// 天气描述
    /// </summary>
    [SwaggerSchema(Description = "天气描述")]
    public string? Summary { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SwaggerSchema(Description = "城市名称")]
    public string? City { get; set; }
}
