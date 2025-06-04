@echo off
REM TypingMaster Container Rebuild Script (Windows Batch)
REM This script stops, removes, and rebuilds the TypingMaster container

setlocal enabledelayedexpansion

set IMAGE_NAME=typingmaster-server
set CONTAINER_NAME=typingmaster-server-container

echo.
echo ==========================================
echo   TypingMaster Container Rebuild Script
echo ==========================================
echo.

REM Change to project root directory
cd /d "%~dp0\.."
echo Working directory: %CD%

echo.
echo [1/5] Stopping existing container...
docker stop %CONTAINER_NAME% 2>nul

echo [2/5] Removing existing container...
docker rm %CONTAINER_NAME% 2>nul

echo [3/5] Removing existing image...
docker rmi %IMAGE_NAME% 2>nul

echo [4/5] Building new image...
docker build -f DockerFiles/Dockerfile -t %IMAGE_NAME% .

if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to build image!
    pause
    exit /b 1
)

echo [5/5] Starting new container...
docker run -d ^
    --name %CONTAINER_NAME% ^
    --network typingmaster-network ^
    -p 8080:80 ^
    -p 8443:443 ^
    %IMAGE_NAME%

if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to start container!
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Container rebuild complete!
echo ========================================
echo.
echo Application available at:
echo   - HTTP:  http://localhost:8080
echo   - HTTPS: https://localhost:8443
echo.
echo Useful commands:
echo   docker logs %CONTAINER_NAME% -f
echo   docker exec -it %CONTAINER_NAME% bash
echo   docker ps
echo.
pause 