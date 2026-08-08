<#
  统一打包脚本：一键打包 后端（dotnet publish → publish.zip）+ 前端（pnpm build → dist.zip）。
  两个产物都落在本仓库 deploy\ 目录下。

  自动检测：后端看 仓库根/src/Api/Api.csproj、前端看 仓库根/../KiteWeb/package.json；
  两个都在 → 先打后台再打前台；只有一个 → 只打它；-Target 指定但不存在 → 报错。
  （仓库根 = 从脚本所在目录向上找到的 KiteServer.sln 所在目录，脚本放仓库哪个子目录都行）

  用法（任意目录均可，路径指向本脚本即可）：
    powershell -File src\Api\pack.ps1                  # 打两端 → deploy\publish.zip + deploy\dist.zip
    powershell -File src\Api\pack.ps1 -Target api      # 只打后端
    powershell -File src\Api\pack.ps1 -Target web      # 只打前端

  后端为「框架依赖」小包（zip 约 20MB，入口 Api/Api.dll，走服务器已装的运行时）。
  ⚠️ 千万别打 win-x64 版（入口 Api.exe，Linux 跑不了）。

  生成文件：deploy\publish.zip（后端）、deploy\dist.zip（前端），均已在 .gitignore 中忽略，不入库。
  说明：zip 用 .NET ZipArchive 生成，条目统一正斜杠路径，服务器 unzip 直接解压；
        兼容 Windows PowerShell 5.1（该版本自带的 Compress-Archive 会写出反斜杠条目，Linux 解压会坏）。
#>
param(
  [ValidateSet('all','api','web')]
  [string]$Target = 'all'
)
$ErrorActionPreference = "Stop"

# ---- 从脚本所在目录向上查找 KiteServer.sln，定位仓库根（脚本放仓库哪个子目录都能跑） ----
$repoRoot = $PSScriptRoot
while (-not (Test-Path (Join-Path $repoRoot 'KiteServer.sln'))) {
  $parent = Split-Path $repoRoot -Parent
  if (-not $parent -or $parent -eq $repoRoot) {
    throw "未找到包含 KiteServer.sln 的仓库根目录（从 $PSScriptRoot 向上一直找到磁盘根），请确认脚本位于 KiteServer 仓库内"
  }
  $repoRoot = $parent
}
$webDir    = Join-Path (Split-Path $repoRoot -Parent) 'KiteWeb'
$deployDir = Join-Path $repoRoot 'deploy'

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

# ---- 把目录内容压成 zip：条目用正斜杠路径（Linux unzip 直接认），index.html/Api.dll 落压缩包根 ----
function Compress-DirToZip([string]$SourceDir, [string]$ZipPath) {
  if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
  $stream  = [System.IO.File]::Open($ZipPath, [System.IO.FileMode]::Create)
  $archive = New-Object System.IO.Compression.ZipArchive($stream, [System.IO.Compression.ZipArchiveMode]::Create)
  try {
    $prefix = $SourceDir.TrimEnd('\').Length + 1
    Get-ChildItem $SourceDir -Recurse -File | ForEach-Object {
      $entry = $_.FullName.Substring($prefix).Replace('\', '/')
      [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
        $archive, $_.FullName, $entry, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
  } finally {
    $archive.Dispose()
    $stream.Dispose()
  }
}

# ---- 自动检测项目存在 ----
$hasApi = Test-Path (Join-Path $repoRoot 'src\Api\Api.csproj')
$hasWeb = Test-Path (Join-Path $webDir 'package.json')
if (-not $hasApi -and -not $hasWeb) { throw "未检测到后端（$repoRoot\src\Api\Api.csproj）或前端（$webDir\package.json），无可打包项目" }
if ($Target -eq 'api' -and -not $hasApi) { throw "-Target api 但未检测到 $repoRoot\src\Api\Api.csproj" }
if ($Target -eq 'web' -and -not $hasWeb) { throw "-Target web 但未检测到 $webDir\package.json" }
$doApi = $hasApi -and ($Target -ne 'web')
$doWeb = $hasWeb -and ($Target -ne 'api')

New-Item -ItemType Directory -Path $deployDir -Force | Out-Null
$summary = @()

# ================= 后端（.NET：dotnet publish → deploy\publish.zip） =================
if ($doApi) {
  Set-Location $repoRoot
  $publishOut = Join-Path $repoRoot 'publish'
  if (Test-Path $publishOut) { Remove-Item $publishOut -Recurse -Force }
  Write-Host "==> [后台] dotnet publish（linux-x64 框架依赖小包，走服务器已装运行时）..." -ForegroundColor Cyan
  dotnet publish src/Api/Api.csproj -c Release -r linux-x64 --self-contained false -o $publishOut
  if ($LASTEXITCODE -ne 0) { throw "[后台] dotnet publish 失败" }
  $zip = Join-Path $deployDir 'publish.zip'
  Write-Host "==> [后台] 压缩发布产物 -> deploy\publish.zip" -ForegroundColor Cyan
  Compress-DirToZip $publishOut $zip
  Remove-Item $publishOut -Recurse -Force   # 产物已进 zip，清理临时发布目录
  $sizeMB = [math]::Round((Get-Item $zip).Length / 1MB, 1)
  Write-Host "==> [后台] 完成：$zip（$sizeMB MB）" -ForegroundColor Green
  $summary += "后台 publish.zip（$sizeMB MB）"
}

# ================= 前端（KiteWeb：pnpm build → dist → deploy\dist.zip） =================
if ($doWeb) {
  Set-Location $webDir
  Write-Host "==> [前台] 构建 KiteWeb（pnpm build）..." -ForegroundColor Cyan
  pnpm build
  if ($LASTEXITCODE -ne 0) { throw "[前台] pnpm build 失败" }
  $distDir = Join-Path $webDir 'dist'
  if (-not (Test-Path (Join-Path $distDir 'index.html'))) { throw "dist\index.html 不存在，构建失败？" }
  $zip = Join-Path $deployDir 'dist.zip'
  Write-Host "==> [前台] 压缩 dist 内容 -> deploy\dist.zip（index.html 落压缩包根）" -ForegroundColor Cyan
  Compress-DirToZip $distDir $zip
  $sizeMB = [math]::Round((Get-Item $zip).Length / 1MB, 1)
  Write-Host "==> [前台] 完成：$zip（$sizeMB MB）" -ForegroundColor Green
  $summary += "前台 dist.zip（$sizeMB MB）"
}

Write-Host ""
Write-Host "🏁 打包完成，产物位于 $deployDir ：" -ForegroundColor Green
$summary | ForEach-Object { Write-Host "   $_" -ForegroundColor Green }
Write-Host ""
Write-Host "下一步：把 deploy 下的 publish.zip / dist.zip 上传到服务器后解压部署即可" -ForegroundColor Yellow
Write-Host "  后端解压后入口 Api.dll（dotnet Api.dll），前端解压后为静态站点（nginx 指向即可）"
