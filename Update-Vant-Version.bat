@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo 子模块更新脚本 (阶段2)
echo ========================================
echo.

set SUBMODULE_PATH_WIN=Assets\Vant-Framework
set SUBMODULE_PATH_GIT=Assets/Vant-Framework

:: 检查子模块目录是否存在
if not exist "%SUBMODULE_PATH_WIN%" (
    echo [错误] 子模块目录不存在: %SUBMODULE_PATH_WIN%
    pause
    exit /b 1
)

echo [步骤1] 检查子模块工作区是否干净...
cd /d "%SUBMODULE_PATH_WIN%"
for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD') do set CURRENT_REF=%%i
echo [信息] 当前引用: %CURRENT_REF%

set HAS_CHANGES=
for /f "tokens=*" %%i in ('git status --porcelain') do set HAS_CHANGES=1
if defined HAS_CHANGES (
    echo [警告] 子模块有未提交的修改，请先处理
    git status --short
    cd /d "%~dp0"
    pause
    exit /b 1
)
cd /d "%~dp0"

echo [步骤2] 更新子模块到远程最新版本...
:: 使用 --checkout，确保子模块落在“父工程记录的 commit (detached HEAD)”而不是停留在某个分支上
git submodule update --init --remote --checkout "%SUBMODULE_PATH_GIT%"
if errorlevel 1 (
    echo [错误] 子模块更新失败
    pause
    exit /b 1
)
echo [完成] 子模块已更新到最新版本

:: 获取更新后的 commit hash
cd /d "%SUBMODULE_PATH_WIN%"
for /f "tokens=*" %%i in ('git rev-parse HEAD') do set LATEST_COMMIT=%%i
for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD') do set FINAL_REF=%%i
cd /d "%~dp0"
echo [信息] 当前引用: %FINAL_REF%
echo [信息] 最新提交: %LATEST_COMMIT:~0,8%

echo.
echo [步骤3] 提交父工程的子模块更新...
git add "%SUBMODULE_PATH_GIT%"
git diff --cached --quiet
if not errorlevel 1 (
    echo [提示] 子模块已是最新版本，无需更新父工程
    pause
    exit /b 0
)
git commit -m "chore: update Vant-Framework submodule to latest"
if errorlevel 1 (
    echo [错误] 提交失败
    pause
    exit /b 1
)
echo [完成] 父工程已提交

echo.
echo [步骤4] 推送父工程的更新...
git push
if errorlevel 1 (
    echo [错误] 推送失败
    pause
    exit /b 1
)
echo [完成] 父工程已推送到远程

echo.
echo [步骤5] 同步本地子模块到父仓库记录的版本...
:: 强制以 checkout 方式同步，保证子模块最终处于父工程记录的 commit（detached HEAD）
git submodule update --init --force --checkout "%SUBMODULE_PATH_GIT%"
if errorlevel 1 (
    echo [警告] 子模块同步失败，但主要流程已完成
) else (
    :: 获取同步后的状态
    cd /d "%SUBMODULE_PATH_WIN%"
    for /f "tokens=*" %%i in ('git rev-parse HEAD') do set SYNCED_COMMIT=%%i
    for /f "tokens=*" %%i in ('git rev-parse --abbrev-ref HEAD') do set SYNCED_REF=%%i
    cd /d "%~dp0"
    echo [完成] 子模块已同步到版本: !SYNCED_COMMIT:~0,8!
    echo [信息] 当前状态: !SYNCED_REF!
)

echo.
echo ========================================
echo 阶段2完成！
echo ========================================
echo 子模块已更新到最新版本
echo 父工程已提交并推送
echo 本地子模块已同步到父仓库记录版本
echo ========================================

pause
