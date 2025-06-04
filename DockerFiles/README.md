# TypingMaster Docker Rebuild Scripts

This directory contains scripts to easily rebuild and restart your TypingMaster Docker containers.

## Available Scripts

### 1. PowerShell Script (Windows) - `rebuild-container.ps1`
**Recommended for Windows users**

```powershell
# Basic rebuild (server container only)
.\DockerFiles\rebuild-container.ps1

# Rebuild with full cleanup
.\DockerFiles\rebuild-container.ps1 -Force

# Rebuild entire stack (all services)
.\DockerFiles\rebuild-container.ps1 -FullStack

# Full rebuild with cleanup
.\DockerFiles\rebuild-container.ps1 -FullStack -Force
```

### 2. Batch Script (Windows) - `rebuild-container.cmd`
**Simple Windows batch script**

```cmd
# Double-click the file or run from command prompt
.\DockerFiles\rebuild-container.cmd
```

### 3. Bash Script (Linux/macOS/WSL) - `rebuild-container.sh`
**For Unix-like systems**

```bash
# Make executable (Linux/macOS only)
chmod +x DockerFiles/rebuild-container.sh

# Basic rebuild
./DockerFiles/rebuild-container.sh

# Rebuild with cleanup
./DockerFiles/rebuild-container.sh --force

# Rebuild entire stack
./DockerFiles/rebuild-container.sh --full-stack

# Full rebuild with cleanup
./DockerFiles/rebuild-container.sh --full-stack --force
```

## What the Scripts Do

### Server Container Only Mode (Default)
1. **Stop** existing container (`typingmaster-server-container`)
2. **Remove** existing container
3. **Remove** existing image (if `--force` flag used)
4. **Build** new image from Dockerfile
5. **Start** new container with proper network and port configuration

### Full Stack Mode (`-FullStack` / `--full-stack`)
1. **Stop** all services defined in docker-compose.yml
2. **Remove** all containers and volumes (if `--force` flag used)
3. **Rebuild** and start all services using docker-compose

## Port Configuration

After running the scripts, your services will be available at:

### Server Container Only
- **HTTP**: http://localhost:8080
- **HTTPS**: https://localhost:8443

### Full Stack
- **PostgreSQL**: localhost:5432
- **Seq Logs**: http://localhost:8081
- **pgAdmin**: http://localhost:8082

## Prerequisites

- Docker Desktop installed and running
- Docker Compose (included with Docker Desktop)
- PowerShell 7+ (for .ps1 script)

## Troubleshooting

### PowerShell Execution Policy
If you get an execution policy error, run:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Network Issues
If the container fails to start due to network issues, create the network manually:
```bash
docker network create typingmaster-network
```

### Port Conflicts
If ports are already in use, modify the port mappings in the scripts or stop conflicting services.

## Useful Docker Commands

```bash
# View container logs
docker logs typingmaster-server-container -f

# Access container shell
docker exec -it typingmaster-server-container bash

# List running containers
docker ps

# List all containers (including stopped)
docker ps -a

# View images
docker images

# Clean up unused resources
docker system prune
```

## Manual Docker Commands

If you prefer to run commands manually:

```bash
# Stop and remove container
docker stop typingmaster-server-container
docker rm typingmaster-server-container

# Build image
docker build -f DockerFiles/Dockerfile -t typingmaster-server .

# Run container
docker run -d \
  --name typingmaster-server-container \
  --network typingmaster-network \
  -p 8080:80 \
  -p 8443:443 \
  typingmaster-server
``` 