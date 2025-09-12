
using Repository.Extensions;
using Application.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Extensions;

/// <summary>
/// Program 扩展方法
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// 配置 Serilog 日志
    /// </summary>
    /// <returns></returns>
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build())
            .CreateLogger();
    }

    /// <summary>
    /// 配置应用程序服务
    /// </summary>
    /// <param name="builder">Web应用程序构建器</param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        // 使用 Serilog 作为日志提供程序
        builder.Host.UseSerilog();

        // 添加配置选项
        builder.Services.AddConfigurationOptions(builder.Configuration);

        // 添加数据库服务
        builder.Services.AddCustomDatabase(builder.Configuration);

        // 添加应用层服务（包括查询服务、Mapster配置等）
        builder.Services.AddApplicationServices();

        // 添加 MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Commands.User.CreateUserCommand).Assembly));

        // 添加 JWT 认证
        builder.Services.AddJwtAuthentication(builder.Configuration);

        // 添加控制器
        builder.Services.AddControllers();

        // 添加 Swagger 文档
        builder.Services.AddSwaggerServices(builder.Configuration);

        // 添加 CORS
        builder.Services.AddCorsServices(builder.Configuration);

        return builder;
    }

    /// <summary>
    /// 配置应用程序管道
    /// </summary>
    /// <param name="app">Web应用程序</param>
    /// <returns></returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // 配置 HTTP 请求管道
        // 使用 Swagger 文档
        app.UseSwaggerMiddleware(app.Configuration);

        // 使用 Serilog 请求日志
        app.UseSerilogRequestLogging();

        // 使用 CORS
        app.UseCorsMiddleware();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    /// <summary>
    /// 添加配置选项
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    private static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));
        services.Configure<FileUploadSettings>(configuration.GetSection(FileUploadSettings.SectionName));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
        services.Configure<HealthCheckSettings>(configuration.GetSection(HealthCheckSettings.SectionName));
        services.Configure<RateLimitSettings>(configuration.GetSection(RateLimitSettings.SectionName));
        services.Configure<SwaggerSettings>(configuration.GetSection(SwaggerSettings.SectionName));

        return services;
    }

    /// <summary>
    /// 添加 CORS 服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    private static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
        if (corsSettings?.Enabled == true)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsSettings.AllowedOrigins)
                          .WithMethods(corsSettings.AllowedMethods)
                          .WithHeaders(corsSettings.AllowedHeaders)
                          .WithExposedHeaders(corsSettings.ExposedHeaders);

                    if (corsSettings.AllowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                });
            });
        }

        return services;
    }

    /// <summary>
    /// 使用 CORS 中间件
    /// </summary>
    /// <param name="app">Web应用程序</param>
    /// <returns></returns>
    private static WebApplication UseCorsMiddleware(this WebApplication app)
    {
        var corsSettings = app.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
        if (corsSettings?.Enabled == true)
        {
            app.UseCors();
        }

        return app;
    }

    /// <summary>
    /// 添加 JWT 认证服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey 未配置");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "KiteServer",
                ValidAudience = "KiteClient",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            // 配置JWT事件
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("JWT认证失败: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Debug("JWT令牌验证成功: {UserId}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Log.Warning("JWT认证质询: {Error}", context.Error);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// 运行应用程序并处理异常
    /// </summary>
    /// <param name="app">Web应用程序</param>
    /// <returns></returns>
    public static async Task RunWithExceptionHandlingAsync(this WebApplication app)
    {
        try
        {
            Log.Information("KiteServer API 服务启动成功");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "KiteServer API 服务运行时发生致命错误");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
