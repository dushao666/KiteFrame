// 全局导入文件 - Repository 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Linq.Expressions;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

// SqlSugar ORM
global using SqlSugar;

// Serilog
global using Serilog;

// 数据库迁移（DbUp）
global using DbUp;
global using DbUp.Engine.Output;

// 项目特定命名空间
global using Shared.Configuration;
global using Domain.Entities;
global using Domain.Entities.Base;
global using Domain.Interfaces;
global using Repository.Extensions;
