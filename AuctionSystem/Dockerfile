# Lightweight ASP.NET Core runtime - used as the final runtime image after building is complete
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Full .NET SDK environment - used for compiling & publishing the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./AuctionSystem/AuctionSystem.csproj"
WORKDIR "/src/AuctionSystem"
RUN dotnet build "./AuctionSystem.csproj" -o /app/build
RUN dotnet publish "./AuctionSystem.csproj" -o /app/publish /p:UseAppHost=false # do not create .exe

FROM base
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuctionSystem.dll"]
