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
}
finally
{
    Log.CloseAndFlush();
}
