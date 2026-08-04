# KiteServer 部署脚本
# 部署路径：Docker Compose（Dockerfile 多阶段构建，直接从源码构建镜像）
# 用法：
#   .\deploy.ps1            # 构建镜像并启动全部服务
#   .\deploy.ps1 -NoBuild   # 复用已有镜像直接启动

param(
    [Parameter(Mandatory=$false)]
    [switch]$NoBuild = $false
)

Write-Host "🚀 开始部署 KiteServer..." -ForegroundColor Green

try {
    Set-Location $PSScriptRoot

    # 检查 .env 文件（敏感凭据来源，禁止入库）
    if (-not (Test-Path (Join-Path $PSScriptRoot ".env"))) {
        Write-Host "❌ 未找到 .env 文件。请先复制 .env.example 为 .env 并修改其中的密码后再部署。" -ForegroundColor Red
        exit 1
    }

    # 停止旧容器
    Write-Host "🛑 停止旧容器..." -ForegroundColor Yellow
    docker compose down

    # 构建并启动
    if ($NoBuild) {
        Write-Host "🐳 复用已有镜像启动..." -ForegroundColor Yellow
        docker compose up -d
    } else {
        Write-Host "🐳 构建镜像并启动..." -ForegroundColor Yellow
        docker compose up -d --build
    }

    if ($LASTEXITCODE -ne 0) {
        throw "Docker 启动失败"
    }

    # 显示服务状态
    docker compose ps

    Write-Host "✅ 部署完成!" -ForegroundColor Green
    Write-Host "API 地址: http://localhost:8080" -ForegroundColor Cyan

} catch {
    Write-Host "❌ 部署失败: $_" -ForegroundColor Red
    exit 1
}
