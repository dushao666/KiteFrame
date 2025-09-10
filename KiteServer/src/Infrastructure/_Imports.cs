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

// 项目特定命名空间
global using Shared.Configuration;
global using Shared.Constants;
global using Shared.Enums;
global using Shared.Models;
global using Domain.Entities.Base;
global using Domain.Interfaces;
global using Application.Common.Interfaces;
global using Infrastructure.Data;
global using Infrastructure.Services;
