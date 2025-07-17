# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TypingMaster is a comprehensive typing tutor application built with .NET 9 and Blazor WebAssembly. The application provides typing courses, practice sessions, speed tests, and progress tracking. It consists of multiple projects organized in a layered architecture pattern.

## Architecture

### Project Structure
- **TypingMaster.Client**: Blazor WebAssembly frontend application
- **TypingMaster.Server**: ASP.NET Core Web API backend
- **TypingMaster.Core**: Domain models and DTOs
- **TypingMaster.Business**: Business logic and services
- **TypingMaster.DataAccess**: Entity Framework Core data layer with PostgreSQL
- **TypingMaster.Shared**: Shared Blazor components and utilities
- **TypingMaster.Tests**: Unit and integration tests

### Key Architectural Patterns
- **Repository Pattern**: Data access abstraction in `TypingMaster.DataAccess/Data`
- **Service Layer**: Business logic in `TypingMaster.Business` with contract interfaces
- **Domain-Driven Design**: Core models in `TypingMaster.Core/Models`
- **AutoMapper**: Object-to-object mapping with profiles in `TypingMaster.Business/Mapping`
- **Dependency Injection**: Service registration in `Program.cs`

### Data Layer
- **Entity Framework Core** with PostgreSQL database
- **Snake case naming convention** for database columns
- **JSON columns** for complex data storage (using PostgreSQL `jsonb`)
- **Migrations** managed in `TypingMaster.DataAccess/Migrations`

## Common Development Commands

### Building and Running
```bash
# Build entire solution
dotnet build

# Run server (API)
dotnet run --project TypingMaster.Server

# Run client (Blazor WebAssembly)
dotnet run --project TypingMaster.Client

# Run tests
dotnet test
```

### Database Management
```bash
# Add new migration
dotnet ef migrations add MigrationName --project TypingMaster.DataAccess --startup-project TypingMaster.Server

# Update database
dotnet ef database update --project TypingMaster.DataAccess --startup-project TypingMaster.Server
```

### Docker Commands
```bash
# Build and run server container
docker build -f DockerFiles/Dockerfile -t typingmaster-server .
docker run -d --name typingmaster-server-container -p 8080:80 -p 8443:443 typingmaster-server

# Use provided rebuild scripts
.\DockerFiles\rebuild-container.ps1          # Windows PowerShell
.\DockerFiles\rebuild-container.cmd          # Windows Batch
./DockerFiles/rebuild-container.sh           # Linux/macOS/WSL
```

## Business Domain

### Core Concepts
- **Accounts**: User accounts with typing history and settings
- **Courses**: Different types of typing training (Beginner, Advanced, Speed Test, All Keys Test)
- **Lessons**: Individual practice sessions within courses
- **Practice Logs**: Historical typing session data
- **Drill Stats**: Detailed statistics for individual typing sessions
- **Key Events**: Granular keystroke data for analysis

### Course Types (TrainingType enum)
- `Course`: Structured learning courses (Beginner, Advanced)
- `AllKeysTest`: Test covering all keyboard keys
- `SpeedTest`: Timed typing speed assessment
- `Game`: Typing games like Word Defender

### Key Services
- **CourseService**: Course management and lesson generation
- **AccountService**: User account management
- **PracticeLogService**: Session tracking and statistics
- **AuthService**: Authentication with JWT tokens
- **TypingTrainer**: Core typing logic and validation

## Database Schema

### Key Tables
- `accounts`: User accounts with JSON settings
- `courses`: Course definitions with JSON configuration
- `practices`: Practice session history
- `drill_stats`: Detailed typing statistics
- `user_profiles`: User profile information
- `login_logs`: Authentication tracking
- `login_credentials`: External identity provider credentials

### Important Relationships
- Account → UserProfile (1:1)
- Account → PracticeLog (1:1)
- Account → Courses (1:many)
- PracticeLog → DrillStats (1:many)

## Configuration

### Key Configuration Files
- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development settings
- `appsettings.Release.json`: Production settings
- Connection strings for PostgreSQL database
- JWT authentication settings
- CORS configuration for client-server communication

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Runtime environment
- Database connection strings
- JWT secret keys
- AWS Cognito settings (for production)

## Testing

### Test Framework
- **xUnit**: Primary testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Assertion library
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for tests

### Test Categories
- Unit tests for business logic
- Integration tests for controllers
- Repository tests with in-memory database
- Client component tests using bUnit

## Authentication & Authorization

### Development Mode
- Uses `MockIdpService` for local development
- JWT tokens for API authentication
- Policy-based authorization with `IdPAuth` policy

### Production Mode
- AWS Cognito integration for identity management
- External identity provider support
- Secure token validation

## Key Development Notes

### AutoMapper Configuration
- Comprehensive mapping profiles in `DomainMapProfile.cs`
- Custom value resolvers for complex mappings
- Queue and Dictionary mapping support

### JSON Serialization
- PostgreSQL `jsonb` columns for complex data
- Custom JSON converters for Entity Framework
- Snake case naming convention for database columns

### Course System
- Factory pattern for course creation (`CourseFactory`)
- Lesson data loaded from JSON files in `Resources/LessonData`
- Phase-based practice progression
- Configurable difficulty settings

### Client-Server Communication
- HTTP client services for API communication
- Blazor component lifecycle management
- Local storage for client-side data persistence

## Common Issues and Solutions

### Database Migrations
- Always run migrations against the correct startup project
- Use snake case naming for new columns
- Test migrations in development before production

### Course Data Loading
- Lesson data files must be included in build output
- JSON deserialization requires proper model mapping
- Course settings validation is critical

### Authentication Flow
- JWT tokens must be properly configured
- CORS settings must allow client domain
- Development vs production IdP service switching