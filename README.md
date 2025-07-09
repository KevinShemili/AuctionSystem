[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=coverage)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=KevinShemili_AuctionSystem&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=KevinShemili_AuctionSystem)

# Vickrey AuctionSystem

## Running the Application

The application is fully containerized, making it easy to run locally without requiring any runtime or development kits installed on the local machine. The only prerequisites are:

- Docker & Docker Compose
- Git

### Steps to Run Locally

1. **Clone the repository** and navigate into the project directory:

    ```bash
    git clone https://github.com/KevinShemili/AuctionSystem.git
    cd AuctionSystem
    ```

2. **Build and start the services** using Docker Compose:

    ```bash
    docker compose up --build
    ```

### What This Command Does

- Builds the application image using the provided `Dockerfile`.
- Starts the following services:
  - **AuctionSystem (Backend)** – accessible at [http://localhost:8080](http://localhost:8080)
  - **PostgreSQL** – the relational database service
  - **pgAdmin UI** – accessible at [http://localhost:5050](http://localhost:5050)
  - **ELK Stack** – for logging and monitoring
  - **Kibana** – accessible at [http://localhost:5601](http://localhost:5601)

---
