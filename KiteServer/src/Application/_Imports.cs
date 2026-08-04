// 全局导入文件 - Application 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Reflection;

// Microsoft Extensions (只引用在类库项目中可用的)
global using Microsoft.Extensions.Configuration;
// global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// MediatR
global using MediatR;

// Mapster
global using Mapster;

// FluentValidation
global using FluentValidation;

// 项目特定命名空间
global using Shared.Models;
global using Shared.Models.Dtos;
global using Shared.Models.User;
global using Shared.Models.Role;
global using Shared.Models.Menu;
global using Shared.Models.Permission;
global using Shared.Models.Monitor;
global using Shared.Enums;
global using Shared.Events;
global using Domain.Entities;
global using Repository;
global using SqlSugar;
global using Infrastructure.Exceptions;
global using Infrastructure.Utilities;
global using Infrastructure.Services;
global using Infrastructure.Extensions;

// Application 命名空间
global using Application.Behaviors;
global using Application.Commands.Auth;
global using Application.Commands.Role;
global using Application.Commands.Menu;
global using Application.Commands.User;
global using Application.DependencyInjection.Mapping;
global using Application.Queries.Permission;
global using Application.Queries.Role;
global using Application.Queries.Menu.Interfaces;
global using Application.Queries.User.Interfaces;
global using Application.Queries.Role.Interfaces;
global using Application.Queries.Permission.Interfaces;
global using Application.Queries.Monitor.Interfaces;

// 类型别名（解决跨命名空间的同名类型冲突）
global using MenuEntity = Domain.Entities.Menu;
global using MenuDto = Shared.Models.Dtos.MenuDto;
// FluentValidation 与 Infrastructure.Exceptions 均定义了 ValidationException，统一指向后者
global using ValidationException = Infrastructure.Exceptions.ValidationException;

// 数据注解
global using System.ComponentModel.DataAnnotations;
