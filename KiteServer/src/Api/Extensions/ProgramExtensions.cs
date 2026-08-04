using Api.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Shared.Constants;

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

        // 添加缓存服务
        builder.Services.AddCacheServices(builder.Configuration);

        // 添加基础设施层服务（HTTP 上下文访问器、当前用户服务等）
        builder.Services.AddInfrastructureServices();

        // 添加应用层服务（包括查询服务、MediatR、校验器、Mapster配置等）
        builder.Services.AddApplicationServices();

        // 配置转发头解析（配合 UseForwardedHeaders 使用）
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // 容器 / 反向代理部署时代理地址通常为动态分配，此处清空默认的环回限制以接受转发头；
            // 若部署拓扑固定，建议改为精确配置 KnownProxies / KnownNetworks
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // 添加 JWT 认证
        builder.Services.AddJwtAuthentication(builder.Configuration);

        // 添加控制器和过滤器
        builder.Services.AddControllers(options =>
        {
            // 注册操作日志过滤器
            options.Filters.Add<Infrastructure.Filters.OperationLogFilter>();
        });

        // 添加 Swagger 文档
        builder.Services.AddSwaggerServices(builder.Configuration);

        // 添加 CORS
        builder.Services.AddCorsServices(builder.Configuration);

        // 添加限流服务
        builder.Services.AddRateLimitServices(builder.Configuration);

        // 添加健康检查
        builder.Services.AddHealthCheckServices(builder.Configuration);

        return builder;
    }

    /// <summary>
    /// 配置应用程序管道
    /// </summary>
    /// <param name="app">Web应用程序</param>
    /// <returns></returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // 执行数据库迁移（DbUp：自动运行所有未执行的迁移脚本）
        DatabaseMigrator.MigrateDatabase(app.Configuration);

        // 全局异常处理（必须位于管道最前端，统一将异常转换为 ApiResult 响应）
        // 使用自定义中间件而非框架 IExceptionHandler 机制（后者在当前运行时版本存在已知的服务解析问题）
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // 解析反向代理转发头（X-Forwarded-For / X-Forwarded-Proto），
        // 使 RemoteIpAddress 与 Scheme 反映真实客户端信息，防止伪造的转发头被直接信任
        app.UseForwardedHeaders();

        // 生产环境启用 HSTS
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // 配置 HTTP 请求管道
        // 使用 Swagger 文档
        app.UseSwaggerMiddleware(app.Configuration);

        // 使用 Serilog 请求日志
        app.UseSerilogRequestLogging(options =>
        {
            // 自定义请求日志格式
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            // 只记录错误和成功的请求
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Information;

            // 增强日志属性
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                }
            };
        });

        // 使用 CORS
        app.UseCorsMiddleware();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        // 限流中间件必须在认证之后，用户维度限流才能读取到认证信息
        app.UseRateLimitMiddleware();

        app.UseAuthorization();

        app.MapControllers();

        // 健康检查端点（/health，供编排系统探活）
        app.UseHealthCheckEndpoints();

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

        // 注册授权策略：每个权限点对应一个同名策略，由 PermissionAuthorizationHandler 基于 RBAC 判定
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy => policy.AddRequirements(new PermissionRequirement(permission)));
            }
        });

        // 权限授权处理器（Scoped：需要注入应用层查询服务）
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

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
