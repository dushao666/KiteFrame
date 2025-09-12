// 全局导入文件 - Application 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Security.Claims;

// Microsoft Extensions (只引用在类库项目中可用的)
global using Microsoft.Extensions.Configuration;
// global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// MediatR
global using MediatR;

// Mapster
global using Mapster;


// 项目特定命名空间
global using Shared.Models;
global using Shared.Models.Dtos;
global using Shared.Enums;
global using Domain.Entities;
global using Repository;
global using SqlSugar;
global using Infrastructure.Exceptions;
global using Infrastructure.Utilities;
global using Infrastructure.Services;
global using Infrastructure.Extensions;

// Application 命名空间
global using Application.Commands.Auth;
global using Application.Queries.Permission;

// 数据注解
global using System.ComponentModel.DataAnnotations;
