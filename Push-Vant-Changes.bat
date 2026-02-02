@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo 子模块变更推送脚本 (阶段1)
echo ========================================
echo.

set SUBMODULE_PATH=Assets\Vant-Framework

:: 检查子模块目录是否存在
if not exist "%SUBMODULE_PATH%" (
    echo [错误] 子模块目录不存在: %SUBMODULE_PATH%
    pause
    exit /b 1
)

:: 进入子模块目录
cd /d "%SUBMODULE_PATH%"
if errorlevel 1 (
    echo [错误] 无法进入子模块目录
    cd /d "%~dp0"
    pause
    exit /b 1
)

:: 检查是否有未提交的修改
git status --porcelain >nul 2>&1
if errorlevel 1 (
    echo [错误] 该目录不是一个有效的 Git 仓库
    cd /d "%~dp0"
    pause
    exit /b 1
)

for /f %%i in ('git status --porcelain') do set HAS_CHANGES=1

if defined HAS_CHANGES (
    echo [信息] 子模块当前状态:
    echo ----------------------------------------
    git status --short
    echo ----------------------------------------
    echo.
    
    :: 输入提交信息
    echo 请输入提交信息:
    set /p COMMIT_MSG=
    if "!COMMIT_MSG!"=="" (
        echo [错误] 提交信息不能为空
        cd /d "%~dp0"
        pause
        exit /b 1
    )
) else (
    echo [信息] 工作区干净，跳过提交步骤
)

:: 输入分支名
echo.
echo 请输入新分支名称:
set /p BRANCH_NAME=
if "%BRANCH_NAME%"=="" (
    echo [错误] 分支名称不能为空
    cd /d "%~dp0"
    pause
    exit /b 1
)

echo.
if defined HAS_CHANGES (
    echo [步骤1] 提交当前修改...
    git add -A
    git commit -m "!COMMIT_MSG!"
    if errorlevel 1 (
        echo [错误] 提交失败
        cd /d "%~dp0"
        pause
        exit /b 1
    )
    echo [完成] 修改已提交
) else (
    echo [步骤1] 跳过提交（无未提交修改）
)

echo.
echo [步骤2] 创建并切换到新分支 '%BRANCH_NAME%'...
git checkout -b "%BRANCH_NAME%"
if errorlevel 1 (
    echo [错误] 创建分支失败
    cd /d "%~dp0"
    pause
    exit /b 1
)
echo [完成] 已切换到新分支

echo.
echo [步骤3] 推送分支到远程...
git push -u origin "%BRANCH_NAME%"
if errorlevel 1 (
    echo [错误] 推送失败
    cd /d "%~dp0"
    pause
    exit /b 1
)
echo [完成] 分支已推送到远程

:: 获取远程仓库URL
for /f "tokens=*" %%i in ('git remote get-url origin') do set REMOTE_URL=%%i

echo.
echo ========================================
echo 阶段1完成！
echo ========================================
echo 分支名称: %BRANCH_NAME%
echo 远程仓库: %REMOTE_URL%
echo.
echo [下一步操作]
echo 1. 请前往代码托管平台创建 Merge Request
echo 2. 将分支 '%BRANCH_NAME%' 合并到 'main' 分支
echo 3. MR 合并完成后，运行 update_submodule_to_latest.bat
echo ========================================

cd /d "%~dp0"
pause
