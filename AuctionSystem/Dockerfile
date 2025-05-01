# -- BULD STAGE --
# Full .NET SDK environment - Restore & Compile & Publish
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Domain
WORKDIR /src
COPY Domain/Domain.csproj Domain/
WORKDIR /src/Domain
RUN dotnet restore Domain.csproj

# Application
WORKDIR /src
COPY Application/Application.csproj Application/
WORKDIR /src/Application
RUN dotnet restore Application.csproj

# Infrastructure
WORKDIR /src
COPY Infrastructure/Infrastructure.csproj Infrastructure/
WORKDIR /src/Infrastructure
RUN dotnet restore Infrastructure.csproj

# WebAPI (AuctionSystem)
WORKDIR /src
COPY AuctionSystem/AuctionSystem.csproj AuctionSystem/
WORKDIR /src/AuctionSystem
RUN dotnet restore AuctionSystem.csproj

# Copy all source files
WORKDIR /src
COPY . .

# Build & Publish
RUN dotnet publish AuctionSystem/AuctionSystem.csproj -c Release -o /app/publish /p:UseAppHost=false # Create DLL, but not .exe

# -- RUNTIME STAGE --
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Run the API
ENTRYPOINT ["dotnet", "AuctionSystem.dll"]