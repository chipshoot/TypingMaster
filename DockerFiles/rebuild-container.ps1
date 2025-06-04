#!/usr/bin/env pwsh

# TypingMaster Container Rebuild Script
# This script stops, removes, and rebuilds the TypingMaster container

param(
    [switch]$FullStack,
    [switch]$Force,
    [string]$ImageName = "typingmaster-server",
    [string]$ContainerName = "typingmaster-server-container"
)

Write-Host "🚀 TypingMaster Container Rebuild Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Change to the project root directory (parent of DockerFiles)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptPath
Set-Location $projectRoot

Write-Host "📁 Working directory: $projectRoot" -ForegroundColor Yellow

if ($FullStack) {
    Write-Host "🔄 Rebuilding full stack with docker-compose..." -ForegroundColor Green
    
    # Stop and remove all services
    Write-Host "⏹️  Stopping all services..." -ForegroundColor Yellow
    docker-compose -f DockerFiles/docker-compose.yml down
    
    if ($Force) {
        Write-Host "🗑️  Removing volumes (--volumes flag)..." -ForegroundColor Yellow
        docker-compose -f DockerFiles/docker-compose.yml down --volumes
        
        Write-Host "🧹 Pruning unused images..." -ForegroundColor Yellow
        docker image prune -f
    }
    
    # Rebuild and start all services
    Write-Host "🔨 Building and starting all services..." -ForegroundColor Green
    docker-compose -f DockerFiles/docker-compose.yml up --build -d
    
    Write-Host "✅ Full stack rebuild complete!" -ForegroundColor Green
    Write-Host "🌐 Services available at:" -ForegroundColor Cyan
    Write-Host "   - Application: https://localhost (when added to compose)" -ForegroundColor White
    Write-Host "   - PostgreSQL: localhost:5432" -ForegroundColor White
    Write-Host "   - Seq Logs: http://localhost:8081" -ForegroundColor White
    Write-Host "   - pgAdmin: http://localhost:8082" -ForegroundColor White
}
else {
    Write-Host "🔄 Rebuilding TypingMaster server container only..." -ForegroundColor Green
    
    # Stop and remove existing container
    Write-Host "⏹️  Stopping container '$ContainerName'..." -ForegroundColor Yellow
    docker stop $ContainerName 2>$null
    
    Write-Host "🗑️  Removing container '$ContainerName'..." -ForegroundColor Yellow
    docker rm $ContainerName 2>$null
    
    # Remove existing image if Force flag is used
    if ($Force) {
        Write-Host "🗑️  Removing image '$ImageName'..." -ForegroundColor Yellow
        docker rmi $ImageName 2>$null
        
        Write-Host "🧹 Pruning unused images..." -ForegroundColor Yellow
        docker image prune -f
    }
    
    # Build new image
    Write-Host "🔨 Building new image '$ImageName'..." -ForegroundColor Green
    docker build -f DockerFiles/Dockerfile -t $ImageName .
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Image built successfully!" -ForegroundColor Green
        
        # Run new container
        Write-Host "🚀 Starting new container '$ContainerName'..." -ForegroundColor Green
        docker run -d `
            --name $ContainerName `
            --network typingmaster-network `
            -p 8080:80 `
            -p 8443:443 `
            $ImageName
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Container rebuild complete!" -ForegroundColor Green
            Write-Host "🌐 Application available at:" -ForegroundColor Cyan
            Write-Host "   - HTTP: http://localhost:8080" -ForegroundColor White
            Write-Host "   - HTTPS: https://localhost:8443" -ForegroundColor White
        }
        else {
            Write-Host "❌ Failed to start container!" -ForegroundColor Red
            exit 1
        }
    }
    else {
        Write-Host "❌ Failed to build image!" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "📋 Useful commands:" -ForegroundColor Cyan
Write-Host "   docker logs $ContainerName -f    # View container logs" -ForegroundColor White
Write-Host "   docker exec -it $ContainerName bash  # Access container shell" -ForegroundColor White
Write-Host "   docker ps                        # List running containers" -ForegroundColor White
Write-Host ""
Write-Host "🔄 Script usage:" -ForegroundColor Cyan
Write-Host "   .\rebuild-container.ps1                    # Rebuild server container only" -ForegroundColor White
Write-Host "   .\rebuild-container.ps1 -FullStack        # Rebuild entire stack" -ForegroundColor White
Write-Host "   .\rebuild-container.ps1 -Force            # Force rebuild (remove images)" -ForegroundColor White
Write-Host "   .\rebuild-container.ps1 -FullStack -Force # Full rebuild with cleanup" -ForegroundColor White 