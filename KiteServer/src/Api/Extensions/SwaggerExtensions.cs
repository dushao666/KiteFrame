namespace Api.Extensions;

/// <summary>
/// Swagger 扩展方法
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// 添加 Swagger 服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetSection(SwaggerSettings.SectionName).Get<SwaggerSettings>() ?? new SwaggerSettings();

        if (!swaggerSettings.Enabled)
        {
            return services;
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // 基本信息配置
            options.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo
            {
                Title = swaggerSettings.Title,
                Version = swaggerSettings.Version,
                Description = swaggerSettings.Description,
                Contact = new OpenApiContact
                {
                    Name = swaggerSettings.Contact.Name,
                    Email = swaggerSettings.Contact.Email,
                    Url = string.IsNullOrEmpty(swaggerSettings.Contact.Url) ? null : new Uri(swaggerSettings.Contact.Url)
                },
                License = new OpenApiLicense
                {
                    Name = swaggerSettings.License.Name,
                    Url = string.IsNullOrEmpty(swaggerSettings.License.Url) ? null : new Uri(swaggerSettings.License.Url)
                }
            });

            // 服务器配置
            foreach (var server in swaggerSettings.Servers)
            {
                if (!string.IsNullOrEmpty(server.Url))
                {
                    options.AddServer(new OpenApiServer
                    {
                        Url = server.Url,
                        Description = server.Description
                    });
                }
            }

            // JWT 认证配置
            if (swaggerSettings.EnableJwtAuth)
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT 授权头使用 Bearer 方案。\r\n\r\n在下面的文本输入框中输入 'Bearer' [空格] 然后输入您的令牌。\r\n\r\n示例: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }

            // XML 注释配置
            if (swaggerSettings.EnableXmlComments)
            {
                // 自动查找 XML 注释文件
                var xmlFiles = new List<string>();
                
                // 添加当前程序集的 XML 文件
                var currentAssembly = Assembly.GetExecutingAssembly();
                var currentXmlFile = Path.Combine(AppContext.BaseDirectory, $"{currentAssembly.GetName().Name}.xml");
                if (File.Exists(currentXmlFile))
                {
                    xmlFiles.Add(currentXmlFile);
                }

                // 添加引用程序集的 XML 文件
                var referencedAssemblies = new[] { "Application", "Domain", "Shared" };
                foreach (var assemblyName in referencedAssemblies)
                {
                    var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
                    if (File.Exists(xmlFile))
                    {
                        xmlFiles.Add(xmlFile);
                    }
                }

                // 添加配置中指定的 XML 文件
                xmlFiles.AddRange(swaggerSettings.XmlCommentFiles.Where(File.Exists));

                // 包含 XML 注释
                foreach (var xmlFile in xmlFiles.Distinct())
                {
                    options.IncludeXmlComments(xmlFile, true);
                }
            }

            // 启用注解
            if (swaggerSettings.EnableAnnotations)
            {
                options.EnableAnnotations();
            }

            // Knife4j 会自动处理大部分过滤和增强功能

            // 忽略过时的 API
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();

            // 自定义架构 ID
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        return services;
    }

    /// <summary>
    /// 使用 Swagger 中间件
    /// </summary>
    /// <param name="app">应用程序</param>
    /// <param name="configuration">配置</param>
    /// <returns></returns>
    public static WebApplication UseSwaggerMiddleware(this WebApplication app, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetSection(SwaggerSettings.SectionName).Get<SwaggerSettings>() ?? new SwaggerSettings();

        if (!swaggerSettings.Enabled)
        {
            return app;
        }

        // 生产环境检查
        if (app.Environment.IsProduction() && !swaggerSettings.EnableInProduction)
        {
            return app;
        }

        app.UseSwagger(options =>
        {
            options.RouteTemplate = $"{swaggerSettings.RoutePrefix}/{{documentName}}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/{swaggerSettings.RoutePrefix}/{swaggerSettings.Version}/swagger.json", $"{swaggerSettings.Title} {swaggerSettings.Version}");
            options.RoutePrefix = swaggerSettings.RoutePrefix;
            options.DocumentTitle = swaggerSettings.DocumentTitle;

            // 基础配置
            options.DefaultModelsExpandDepth(-1); // 不展开模型
            options.DefaultModelExpandDepth(2);   // 模型展开深度
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // 不自动展开文档
            options.EnableDeepLinking();          // 启用深度链接
            options.DisplayOperationId();        // 显示操作 ID
            options.DisplayRequestDuration();    // 显示请求持续时间
        });

        // 启用 Knife4j UI
        app.UseKnife4UI(options =>
        {
            options.RoutePrefix = string.Empty; // 在根路径提供 Knife4j UI
            options.SwaggerEndpoint($"/{swaggerSettings.Version}/api-docs", $"{swaggerSettings.Title} {swaggerSettings.Version}");
        });

        // 映射 Swagger 端点为 Knife4j 格式
        app.MapSwagger("{documentName}/api-docs", options =>
        {
            options.SerializeAsV2 = true;
        });

        return app;
    }
}
