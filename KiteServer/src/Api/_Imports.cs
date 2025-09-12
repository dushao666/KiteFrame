// 全局导入文件 - Api 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using Microsoft.OpenApi.Models;
global using System.Reflection;
global using IGeekFan.AspNetCore.Knife4jUI;
// ASP.NET Core
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Controllers;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;

// Swagger/OpenAPI
global using Swashbuckle.AspNetCore.Annotations;

// Serilog
global using Serilog;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// MediatR
global using MediatR;

// 项目特定命名空间
global using Shared.Configuration;
global using Api.Extensions;
