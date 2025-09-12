// 全局导入文件 - Infrastructure 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Text;
global using System.Text.Json;
global using System.Linq.Expressions;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// ASP.NET Core
global using Microsoft.AspNetCore.Http;

// SqlSugar ORM
global using SqlSugar;

// 缓存
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.StackExchangeRedis;

// Redis
global using StackExchange.Redis;

// 并发集合
global using System.Collections.Concurrent;

// 项目引用
global using Shared.Configuration;
global using Infrastructure.Services;

// 项目特定命名空间
global using Infrastructure.Services;

// Infrastructure 扩展方法和工具类
global using Infrastructure.Extensions;
global using Infrastructure.Utilities;
global using Infrastructure.Exceptions;
