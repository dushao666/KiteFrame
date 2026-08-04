// 全局导入文件 - UnitTests 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System.Text.RegularExpressions;

// 测试框架
global using Xunit;

// 校验与映射
global using FluentValidation;
global using Mapster;

// 数据库
global using SqlSugar;
global using Microsoft.Extensions.Configuration;

// 项目特定命名空间
global using Shared.Configuration;
global using Shared.Enums;
global using Shared.Models.Dtos;
global using Shared.Models.User;
global using Domain.Entities;
global using Repository;
global using Repository.Extensions;
global using Repository.Migrations;
global using Infrastructure.Utilities;
global using Application.Commands.Auth;
global using Application.Commands.User;
global using Application.Validators.Auth;
global using Application.DependencyInjection.Mapping;
