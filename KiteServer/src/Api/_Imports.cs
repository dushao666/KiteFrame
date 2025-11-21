// 全局导入文件 - Api 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Threading.RateLimiting;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Text;
global using Microsoft.OpenApi.Models;
global using System.Reflection;
global using IGeekFan.AspNetCore.Knife4jUI;
// ASP.NET Core
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Controllers;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.RateLimiting;

// Swagger/OpenAPI
global using Swashbuckle.AspNetCore.Annotations;

// Serilog
global using Serilog;
global using Serilog.Events;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// MediatR
global using MediatR;
global using Shared.Attributes;
global using Shared.Events;

// 项目特定命名空间
global using Shared.Configuration;
global using Shared.Models;
global using Shared.Models.Dtos;
global using Shared.Enums;
global using Application.Commands.Auth;
global using Application.DependencyInjection;
global using Repository.Extensions;
global using Infrastructure.Extensions;
global using Infrastructure.Exceptions;
global using Api.Extensions;

// JWT
global using Microsoft.IdentityModel.Tokens;
global using System.Security.Claims;
global using Application.Commands.User;
global using Application.Commands.Role;
global using Application.Commands.Menu;
global using Application.Queries.User;
global using Application.Queries.Role;
global using Application.Queries.Permission;
global using Application.Queries.Monitor;
global using Application.Queries.Menu.Interfaces;
global using Application.Queries.User.Interfaces;
global using Application.Queries.Role.Interfaces;
global using Application.Queries.Permission.Interfaces;
global using Application.Queries.Monitor.Interfaces;
global using Shared.Models.User;
global using Shared.Models.Role;
global using Shared.Models.Menu;
global using Shared.Models.Permission;
global using Shared.Models.Monitor;
global using MenuDto = Shared.Models.Dtos.MenuDto;
global using OnlineUserDto = Shared.Models.Dtos.OnlineUserDto;
global using LoginLogDto = Shared.Models.Dtos.LoginLogDto;
global using OperationLogDto = Shared.Models.Dtos.OperationLogDto;
