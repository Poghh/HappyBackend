# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY Happy.Backend.sln .
COPY Happy.Backend.Api/Happy.Backend.Api.csproj Happy.Backend.Api/
COPY Happy.Backend.Application/Happy.Backend.Application.csproj Happy.Backend.Application/
COPY Happy.Backend.Domain/Happy.Backend.Domain.csproj Happy.Backend.Domain/
COPY Happy.Backend.Infrastructure/Happy.Backend.Infrastructure.csproj Happy.Backend.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish Happy.Backend.Api/Happy.Backend.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Happy.Backend.Api.dll"]
