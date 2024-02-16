# api-gtw-ted
This project, developed in .NET 6, leverages SQLServer as its core database management system. Designed with robustness and scalability in mind, `api-gtw-ted` serves as a comprehensive solution for managing data-intensive applications. The instructions below guide you through the process of setting up your development environment. This includes configuring the SQLServer database using Docker, initializing the Entity Framework, and preparing the project for Docker deployment.

---

## Prerequisites
Before starting, it is necessary to have the .NET 6 SDK installed on your computer.
The SDK can be obtained through the [official Microsoft website](https://dotnet.microsoft.com/download).

---

## Appsettings configuration
appsettings.Local.example.json within this file contains explanations about the environment variables.
Copy the appsettings.Local.example.json file to appsettings.Local.json

---

## Database Configuration
Here are the steps to create and configure the SQLServer database:

#### Downloading the Image and Installing SQLServer:
```
docker pull mcr.microsoft.com/mssql/server:2019-latest

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Hub@1234" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```
By executing these commands, the container with SQLServer will be started.

---

## Entity Framework Configuration
To configure the Entity Framework to interact with the PostgreSQL database, execute the following commands:

### Add Migration:

Execute the command below:

```
dotnet ef migrations add InitialCreate
```

### Update the Database:
Then, update the database with:
```
dotnet ef database update
```

---

## Project Configuration
To build and run the application using Docker, follow these steps:
### Building the Docker Image:
Build the Docker image using the following command:
```
docker build --no-cache -t ted-image .
```
### Running the Docker Container:
Run the Docker container with the following command:
```
docker run -d -p 8080:80 -p 8081:443 --name ted-container ted-image
```
The Dockerfile is located at the root of the project.

---
## Verifying if the API is Working
Access: [http://localhost:8080/health](http://localhost:8080/health)

---

## Open Swagger
Access: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

