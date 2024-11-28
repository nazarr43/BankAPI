# BankAPI

## Project Description

BankAPI is a secure, scalable web API system designed for a banking application. It includes essential features such as user registration, login with JWT tokens, and role-based access control for employees and clients. The system is built using **C#**, **ASP.NET Core Web API**, and **Entity Framework Core**, ensuring efficient data management and scalability with **PostgreSQL**.

Key functionalities include:
- **User Authentication & Authorization**: JWT tokens are used to authenticate and authorize users based on roles.
- **Transaction Handling**: Support for handling multiple currency balances and various types of transactions.
- **Data Import**: Integrates with external partners, supporting data imports in both **XML** and **JSON** formats.
- **Validation & Testing**: Business rules are enforced using **FluentValidation**, and unit tests are written using **xUnit**, **Moq**, and **FluentAssertions** to verify core functionalities.
- **API Architecture**: The project follows **Clean Architecture** with **Domain-Driven Design (DDD)** principles and emphasizes **SOLID** principles for maintainable code.

### Key Features:
- **RESTful API** for seamless integration with clients.
- **JWT Authentication** for secure login and role-based access control.
- **Data Management** using **PostgreSQL** and **Entity Framework Core**.
- **Caching** with **Redis** for performance optimization.
- **Message Queue** using **Kafka** for real-time data processing.
- **Reverse Proxy** functionality powered by **YARP (Yet Another Reverse Proxy)**.
- **Integration** with external partners via **XML** and **JSON** data formats.
- **Business Logic Validation** with **FluentValidation**.
- **Monitoring and Logging** using **ELK Stack**, **Grafana**, and **Prometheus** for observability and performance tracking.

## Technologies Used

- **C#**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **xUnit**
- **Moq**
- **Fluent Assertions**
- **JWT (JSON Web Tokens)**
- **PostgreSQL**
- **RESTful API**
- **LINQ**
- **Clean Architecture with DDD (Domain-Driven Design)**
- **CI/CD**
- **Docker**
- **Dapper**
- **Kafka**
- **Redis**
- **ELK Stack** (Elasticsearch, Logstash, Kibana)
- **OpenTelemetry**
- **Grafana**
- **Prometheus**
- **Reverse Proxy (YARP)**

## Architecture

This project is designed using a **microservices architecture**, which allows for modularity, scalability, and flexibility. Each service can evolve independently while communicating via well-defined API contracts, using **Kafka** for messaging between services. The architecture follows **Clean Architecture** principles, ensuring separation of concerns and ease of maintenance.

### Key Components:
- **API Gateway**: Serves as a single entry point for all client requests.
- **User Service**: Manages user registration, authentication, and authorization.
- **Transaction Service**: Handles transaction operations such as deposits, withdrawals, and balance updates.
- **Notification Service**: Sends notifications to users based on predefined events.
- **Integration Service**: Imports data from external systems in various formats (XML, JSON).
- **Logging & Monitoring**: Utilizes **ELK Stack**, **Grafana**, **Prometheus**, and **OpenTelemetry** for comprehensive monitoring, logging, and observability.

## How to Run Locally

### Prerequisites
- **.NET Core 8.0** or later.
- **PostgreSQL** or any other supported database.
- **Docker** (for containerization).
- **Kafka** (for messaging between services).
- **Redis** (for caching).
- **YARP** for reverse proxy functionality.
