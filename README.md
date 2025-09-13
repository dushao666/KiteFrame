# KiteFrame - 前后端一体化脚手架

KiteFrame 是一个包含前端管理界面与后端 API 的一体化项目，旨在提供开箱即用的企业级开发脚手架。

- 前端：`KiteWeb/` 基于 Vue 3 + Vite + Element Plus（pure-admin 精简版 i18n）。
- 后端：`KiteServer/` 基于 .NET 9 + SqlSugar + MySQL，支持 Redis、Serilog、JWT、Mapster 等。

> 详细说明请分别阅读：
> - 前端文档：`KiteWeb/README.md`
> - 后端文档：`KiteServer/README.md`

---

## 目录结构

```
KiteFrame/
├── KiteWeb/                                 # 前端 (Vue 3 + Vite)
│   ├── package.json                         # 脚本命令、依赖、engines（要求 Node 20/22 + pnpm ≥ 9）
│   ├── vite.config.ts                       # Vite 配置（含 /api -> :5153 代理）
│   ├── .env*                                # 多环境变量（development/production/staging）
│   ├── src/                                 # 前端源码
│   │   ├── main.ts                          # 应用入口，注册路由、Pinia、i18n、ElementPlus 等
│   │   ├── api/                             # 接口封装
│   │   ├── assets/                          # 静态资源、图标、字体等
│   │   ├── components/                      # 通用业务组件（含权限组件 `ReAuth`/`RePerms`）
│   │   ├── directives/                      # 自定义指令
│   │   ├── layout/                          # 布局、导航、菜单等
│   │   ├── plugins/                         # 插件装配（i18n、ElementPlus、ECharts 等）
│   │   ├── router/                          # 路由配置
│   │   ├── store/                           # Pinia 状态管理
│   │   ├── style/                           # 全局样式（reset.scss、index.scss、tailwind.css 等）
│   │   ├── utils/                           # 工具方法、响应式存储等
│   │   └── views/                           # 业务页面（登录、工作台、示例页等）
│   └── README.md                            # 前端详细说明（pure-admin 精简版）
├── KiteServer/                              # 后端 (.NET 9)
│   ├── src/                                 # 分层：Api / Application / Domain / Infrastructure / Repository / Shared
│   │   ├── Api/                             # Web API 层
│   │   │   ├── Program.cs                   # 入口，Serilog 配置、服务与管道装配
│   │   │   ├── appsettings.json             # 基础配置（按环境覆盖）
│   │   │   ├── appsettings.Development.json # 开发环境配置（含连接串/Serilog/CORS/Swagger 等）
│   │   │   └── appsettings.Production.json  # 生产环境配置
│   │   ├── Application/                     # 应用服务层（CQRS、DTO、用例）
│   │   ├── Domain/                          # 领域层（实体、值对象、领域服务）
│   │   ├── Infrastructure/                  # 基础设施（EF/SqlSugar、缓存、日志、第三方集成）
│   │   ├── Repository/                      # 仓储实现（数据访问抽象）
│   │   └── Shared/                          # 共享内核（通用模型、结果类型、常量）
│   ├── deploy/                              # 部署编排
│   │   ├── Dockerfile                       # 后端镜像构建（基于 mcr dotnet 9）
│   │   ├── docker-compose.yml               # 一键编排（API/MySQL/Redis/Nginx）
│   │   └── deploy.ps1                       # 发布与启动脚本（支持 -Build、-Environment）
│   └── README.md                            # 后端详细说明
└── README.md                                # 根级统一说明（当前文件）
```

---

## 技术栈概览

- 前端
  - Vue 3、TypeScript、Vite 7
  - Element Plus、Pinia、Vue Router、Vue I18n
  - Tailwind CSS、ECharts、@pureadmin 生态
- 后端
  - .NET 9 Web API
  - SqlSugar、MySQL、Redis
  - Serilog、JWT、Mapster、FluentValidation、MediatR
  - 架构：洋葱架构 + CQRS + Repository/UnitOfWork + DDD

---

## 快速开始（本地开发）

### 前置要求
- Node.js 20.x 或 22.x，包管理器建议使用 pnpm ≥ 9（`KiteWeb/package.json` 中有 engines 限制）。
- .NET SDK 9.0（后端）。
- MySQL 8.0+（建议本地或容器）与 Redis（可选）。

### 1. 启动后端
1) 配置数据库与 Redis（可选）
- 修改 `KiteServer/src/Api/appsettings.Development.json`
  - `ConnectionStrings.DefaultConnection`
  - `ConnectionStrings.Redis` 或 `RedisSettings`
2) 运行 API（默认端口 5153，Swagger 在开发环境开启）
```
dotnet run --project KiteServer/src/Api
```
访问接口文档（开发）：`http://localhost:5153/openapi`（若在 `SwaggerSettings` 中启用）

### 2. 启动前端
1) 安装依赖
```
cd KiteWeb
pnpm install
```
2) 启动开发服务器
```
pnpm dev
```
- Vite 代理已在 `KiteWeb/vite.config.ts` 配置：`/api -> http://localhost:5153`
- 浏览器访问：`http://localhost:5173`

### 本地联调说明
- 跨域白名单：`appsettings.Development.json` 中 `CorsSettings.AllowedOrigins` 已包含常见本地端口（如 5173）。
- 如需修改代理或端口，请同步调整 Vite 与 API 端口配置。

---

## 构建与部署

### 前端构建
```
cd KiteWeb
pnpm build
```
- 产物输出到 `KiteWeb/dist/`。
- 可使用 `vite preview` 或接入 Nginx/静态服务器部署。

### 后端发布
```
# Release 构建与发布到本地目录
cd KiteServer
pwsh ./deploy/deploy.ps1 -Build -Environment Production
```
- 脚本会执行 `dotnet publish` 并可选择启动 Docker 编排。
- 生产环境配置可放在 `KiteServer/src/Api/appsettings.Production.json`。

### Docker 一键编排
```
cd KiteServer/deploy
# 首次或变更后建议带 --build
docker-compose up -d --build
```
- 组件：API（映射 8080/8081）、MySQL（3306）、Redis（6379）、Nginx（80/443）。
- 卷：日志 `./logs`、上传 `./uploads`、MySQL/Redis 数据卷。
- 如需自定义域名/证书，修改 `deploy/nginx/nginx.conf` 与 `deploy/nginx/ssl/`。

---

## 环境与配置
- 前端环境变量：`KiteWeb/.env*`，`VITE_PUBLIC_PATH`、`VITE_PORT`、`VITE_CDN`、`VITE_COMPRESSION` 等。
- 后端配置文件：`KiteServer/src/Api/appsettings.json`、`appsettings.Development.json`、`appsettings.Production.json`。
- Swagger 开关与服务器列表：`SwaggerSettings`。
- CORS、缓存、限流、文件上传等均在 `appsettings.*.json` 中集中管理。

---


## 常见问题
- 端口冲突：
  - 前端默认 `5173`，后端默认 `5153`（开发），Docker 暴露 `8080/8081`。
  - 冲突时修改 `vite.config.ts` 与 `appsettings.*.json`。
- 跨域问题：
  - 确认后端 `CorsSettings.Enabled=true` 且包含前端来源。
  - 前端通过 `/api` 代理到后端，路径重写保持 `/api` 前缀。
- 数据库初始化：
  - 确认连接串正确，必要时在 `DatabaseSettings` 中开启 CodeFirst/建表策略。

---

## 许可证
- 前端 `KiteWeb/` 遵循其自带 `LICENSE`（MIT）。
- 后端 `KiteServer/` 使用 MIT License。

如需进一步定制与集成，请分别参考：
- `KiteWeb/README.md`
- `KiteServer/README.md`
