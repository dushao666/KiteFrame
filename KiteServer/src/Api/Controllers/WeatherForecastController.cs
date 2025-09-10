namespace Api.Controllers;

/// <summary>
/// 天气预报控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("天气预报相关接口")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取天气预报
    /// </summary>
    /// <param name="days">预报天数</param>
    /// <returns>天气预报列表</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "获取天气预报",
        Description = "获取指定天数的天气预报信息",
        OperationId = "GetWeatherForecast",
        Tags = new[] { "WeatherForecast" }
    )]
    [SwaggerResponse(200, "获取成功", typeof(IEnumerable<WeatherForecast>))]
    [SwaggerResponse(400, "请求参数错误")]
    [SwaggerResponse(500, "服务器内部错误")]
    public ActionResult<IEnumerable<WeatherForecast>> Get(
        [FromQuery, SwaggerParameter("预报天数，范围1-10天", Required = false)]
        [Range(1, 10, ErrorMessage = "预报天数必须在1-10之间")]
        int days = 5)
    {
        try
        {
            _logger.LogInformation("获取 {Days} 天的天气预报", days);

            var forecasts = Enumerable.Range(1, days).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(forecasts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取天气预报时发生错误");
            return StatusCode(500, "服务器内部错误");
        }
    }
}
