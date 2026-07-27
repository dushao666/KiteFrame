# KiteServer 部署脚本
# PowerShell 脚本用于自动化部署

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Production",
    
    [Parameter(Mandatory=$false)]
    [switch]$Build = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean = $false
)

Write-Host "🚀 开始部署 KiteServer..." -ForegroundColor Green
Write-Host "环境: $Environment" -ForegroundColor Yellow

# 设置变量
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ApiProject = Join-Path $ProjectRoot "src\Api\Api.csproj"
$PublishPath = Join-Path $PSScriptRoot "publish"

try {
    # 清理旧的发布文件
    if ($Clean -and (Test-Path $PublishPath)) {
        Write-Host "🧹 清理旧的发布文件..." -ForegroundColor Yellow
        Remove-Item -Path $PublishPath -Recurse -Force
    }

    # 构建项目
    if ($Build) {
        Write-Host "🔨 构建项目..." -ForegroundColor Yellow
        Set-Location $ProjectRoot
        dotnet clean
        dotnet restore
        dotnet build --configuration Release
        
        if ($LASTEXITCODE -ne 0) {
            throw "构建失败"
        }
    }

    # 发布项目
    Write-Host "📦 发布项目..." -ForegroundColor Yellow
    dotnet publish $ApiProject `
        --configuration Release `
        --output $PublishPath `
        --no-build:$(!$Build) `
        --verbosity minimal

    if ($LASTEXITCODE -ne 0) {
        throw "发布失败"
    }

    # 复制配置文件
    Write-Host "📋 复制配置文件..." -ForegroundColor Yellow
    $ConfigSource = Join-Path $ProjectRoot "src\Api\appsettings.$Environment.json"
    $ConfigTarget = Join-Path $PublishPath "appsettings.json"
    
    if (Test-Path $ConfigSource) {
        Copy-Item $ConfigSource $ConfigTarget -Force
    }

    # 启动 Docker 容器
    Write-Host "🐳 启动 Docker 容器..." -ForegroundColor Yellow
    Set-Location $PSScriptRoot
    docker-compose down
    docker-compose up -d --build

    if ($LASTEXITCODE -ne 0) {
        throw "Docker 启动失败"
    }

    Write-Host "✅ 部署完成!" -ForegroundColor Green
    Write-Host "API 地址: http://localhost:8080" -ForegroundColor Cyan
    Write-Host "文档地址: http://localhost:8080" -ForegroundColor Cyan

} catch {
    Write-Host "❌ 部署失败: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location $ProjectRoot
}
