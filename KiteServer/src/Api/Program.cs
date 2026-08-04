// 配置 Serilog
ProgramExtensions.ConfigureSerilog();

try
{
    Log.Information("启动 KiteServer API 服务");

    var builder = WebApplication.CreateBuilder(args);

    // 配置服务
    builder.ConfigureServices();

    var app = builder.Build();

    // 配置管道
    app.ConfigurePipeline();

    // 运行应用程序
    await app.RunWithExceptionHandlingAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "KiteServer API 服务启动失败");

    // 启动失败（含数据库迁移失败）必须以非零退出码结束，
    // K8s / systemd 等编排系统的 on-failure 重启与告警策略才能生效（快速失败）
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Program 部分类声明，供集成测试 WebApplicationFactory 引用
/// </summary>
public partial class Program { }
