# Start by using the .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
# Copy the solution file and the project files
COPY WebAPI/WebAPI.sln ./
COPY WebAPI/WebAPI/WebAPI.csproj ./WebAPI/WebAPI/
COPY WebAPI/WebAPI.Presentation/WebAPI.Presentation.csproj ./WebAPI/WebAPI.Presentation/
COPY WebAPI/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj ./WebAPI/WebAPI.IntegrationTests/
COPY WebAPI/WebAPI.Tests/WebAPI.Tests.csproj ./WebAPI/WebAPI.Tests/
COPY WebAPI/WebAPI.Infrastructure/WebAPI.Infrastructure.csproj ./WebAPI/WebAPI.Infrastructure/
COPY WebAPI/WebAPI.Domain/WebAPI.Domain.csproj ./WebAPI/WebAPI.Domain/
COPY WebAPI/WebAPI.Application/WebAPI.Application.csproj ./WebAPI/WebAPI.Application/
COPY WebAPI/DataMigrationTool/DataMigrationTool.csproj ./WebAPI/DataMigrationTool/

# Restore dependencies
RUN dotnet restore WebAPI/WebAPI/WebAPI.csproj
RUN dotnet restore WebAPI/WebAPI.Presentation/WebAPI.Presentation.csproj
RUN dotnet restore WebAPI/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj
RUN dotnet restore WebAPI/WebAPI.Tests/WebAPI.Tests.csproj
RUN dotnet restore WebAPI/WebAPI.Infrastructure/WebAPI.Infrastructure.csproj
RUN dotnet restore WebAPI/WebAPI.Domain/WebAPI.Domain.csproj
RUN dotnet restore WebAPI/WebAPI.Application/WebAPI.Application.csproj
RUN dotnet restore WebAPI/DataMigrationTool/DataMigrationTool.csproj
# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish WebAPI/WebAPI.sln -c Release -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Entry point
ENTRYPOINT ["dotnet", "WebAPI.dll"]
