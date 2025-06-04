#!/bin/bash

# TypingMaster Container Rebuild Script (Bash)
# This script stops, removes, and rebuilds the TypingMaster container

set -e  # Exit on any error

# Configuration
IMAGE_NAME="typingmaster-server"
CONTAINER_NAME="typingmaster-server-container"
FORCE=false
FULL_STACK=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --force|-f)
            FORCE=true
            shift
            ;;
        --full-stack|-s)
            FULL_STACK=true
            shift
            ;;
        --help|-h)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  --force, -f       Force rebuild (remove images and volumes)"
            echo "  --full-stack, -s  Rebuild entire stack using docker-compose"
            echo "  --help, -h        Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo "🚀 TypingMaster Container Rebuild Script"
echo "========================================="

# Change to project root directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

echo "📁 Working directory: $PROJECT_ROOT"

if [ "$FULL_STACK" = true ]; then
    echo "🔄 Rebuilding full stack with docker-compose..."
    
    # Stop and remove all services
    echo "⏹️  Stopping all services..."
    docker-compose -f DockerFiles/docker-compose.yml down
    
    if [ "$FORCE" = true ]; then
        echo "🗑️  Removing volumes..."
        docker-compose -f DockerFiles/docker-compose.yml down --volumes
        
        echo "🧹 Pruning unused images..."
        docker image prune -f
    fi
    
    # Rebuild and start all services
    echo "🔨 Building and starting all services..."
    docker-compose -f DockerFiles/docker-compose.yml up --build -d
    
    echo "✅ Full stack rebuild complete!"
    echo "🌐 Services available at:"
    echo "   - PostgreSQL: localhost:5432"
    echo "   - Seq Logs: http://localhost:8081"
    echo "   - pgAdmin: http://localhost:8082"
else
    echo "🔄 Rebuilding TypingMaster server container only..."
    
    # Stop and remove existing container
    echo "⏹️  Stopping container '$CONTAINER_NAME'..."
    docker stop "$CONTAINER_NAME" 2>/dev/null || true
    
    echo "🗑️  Removing container '$CONTAINER_NAME'..."
    docker rm "$CONTAINER_NAME" 2>/dev/null || true
    
    # Remove existing image if Force flag is used
    if [ "$FORCE" = true ]; then
        echo "🗑️  Removing image '$IMAGE_NAME'..."
        docker rmi "$IMAGE_NAME" 2>/dev/null || true
        
        echo "🧹 Pruning unused images..."
        docker image prune -f
    fi
    
    # Build new image
    echo "🔨 Building new image '$IMAGE_NAME'..."
    docker build -f DockerFiles/Dockerfile -t "$IMAGE_NAME" .
    
    # Run new container
    echo "🚀 Starting new container '$CONTAINER_NAME'..."
    docker run -d \
        --name "$CONTAINER_NAME" \
        --network typingmaster-network \
        -p 8080:80 \
        -p 8443:443 \
        "$IMAGE_NAME"
    
    echo "✅ Container rebuild complete!"
    echo "🌐 Application available at:"
    echo "   - HTTP: http://localhost:8080"
    echo "   - HTTPS: https://localhost:8443"
fi

echo ""
echo "📋 Useful commands:"
echo "   docker logs $CONTAINER_NAME -f    # View container logs"
echo "   docker exec -it $CONTAINER_NAME bash  # Access container shell"
echo "   docker ps                        # List running containers"
echo ""
echo "🔄 Script usage:"
echo "   ./rebuild-container.sh                # Rebuild server container only"
echo "   ./rebuild-container.sh --full-stack  # Rebuild entire stack"
echo "   ./rebuild-container.sh --force       # Force rebuild (remove images)"
echo "   ./rebuild-container.sh -s -f         # Full rebuild with cleanup" 