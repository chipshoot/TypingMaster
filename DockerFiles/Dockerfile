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
EXPOSE 8083 
EXPOSE 8084

# Copy the published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8083;https://+:8084
ENV ASPNETCORE_ENVIRONMENT=Release
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=devcertpassword

# Install OpenSSL
RUN apt-get update && apt-get install -y openssl

# Create the https directory
RUN mkdir -p /https

# Generate self-signed certificate with OpenSSL
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /https/aspnetapp.key -out /https/aspnetapp.crt \
    -subj "/CN=localhost" && \
    openssl pkcs12 -export -out /https/aspnetapp.pfx \
    -inkey /https/aspnetapp.key -in /https/aspnetapp.crt \
    -passout pass:devcertpassword

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser
RUN chown -R appuser:appuser /https
RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "TypingMaster.Server.dll"] 