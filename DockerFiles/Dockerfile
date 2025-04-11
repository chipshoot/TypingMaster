# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["TypingMaster.Server/TypingMaster.Server.csproj", "TypingMaster.Server/"]
COPY ["TypingMaster.Business/TypingMaster.Business.csproj", "TypingMaster.Business/"]
COPY ["TypingMaster.DataAccess/TypingMaster.DataAccess.csproj", "TypingMaster.DataAccess/"]
RUN dotnet restore "TypingMaster.Server/TypingMaster.Server.csproj"

# Copy the rest of the code
COPY . .

# Build the application
WORKDIR "/src/TypingMaster.Server"
RUN dotnet build "TypingMaster.Server.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TypingMaster.Server.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser
RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "TypingMaster.Server.dll"] 