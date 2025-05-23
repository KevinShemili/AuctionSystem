[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=coverage)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)

# Auction System

# Setting Up The Development Environment (.NET Core 7 + PostgreSQL 17.4 + PGAdmin)

## Configuration Setup

The following configuration files need to be created and filled with own local configurations:

- `.env`
- `appsettings.json`
- `appsettings.Docker.json`

---

### `.env`

This file should reside in the same directory as the `docker-compose.yml` file. There should be defined the environment variables required by PostgreSQL & PGAdmin containers.

```env
POSTGRES_USER=...
POSTGRES_PASSWORD=...
PGADMIN_EMAIL=...
PGADMIN_PASSWORD=...
```

---

### `appsettings.json`

Should be based on the structure of `appsettings.Template.json`. The connection string defined here should apply migrations to the containerized PostgreSQL. To that end, the host should be set to localhost, and the port should be set to the host port.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=hostport;Database=...;Username=...;Password=..."
  }
```

---

### `appsettings.Docker.json`

Should be based on the structure of `appsettings.Template.json`. The connection string defined here will be used by the application during runtime, therefore inside of the container. Thus, the containers communicate with their own ports. To that end, the host should be changed to to the name of the PostgreSQL container, and the port should be set to the container port.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=containername;Port=containerport;Database=...;Username=...;Password=..."
  }
```
