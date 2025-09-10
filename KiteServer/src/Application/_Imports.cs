// 全局导入文件 - Application 项目
// 这个文件中的 using 语句会自动应用到项目中的所有 C# 文件

// .NET 基础命名空间
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;

// Microsoft Extensions (只引用在类库项目中可用的)
// global using Microsoft.Extensions.Configuration;
// global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
// global using Microsoft.Extensions.Options;

// MediatR
global using MediatR;

// AutoMapper
global using AutoMapper;

// FluentValidation
global using FluentValidation;

// 项目特定命名空间
global using Shared.Configuration;
global using Shared.Constants;
global using Shared.Enums;
global using Shared.Models;
global using Domain.Entities.Base;
global using Domain.Interfaces;
// global using Infrastructure.Data;
global using Application.Common.Interfaces;
global using Application.Common.Models;
