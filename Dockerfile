# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies (cached layer)
COPY src/TakeHomeChallenge.Domain/TakeHomeChallenge.Domain.csproj src/TakeHomeChallenge.Domain/
COPY src/TakeHomeChallenge.Application/TakeHomeChallenge.Application.csproj src/TakeHomeChallenge.Application/
COPY src/TakeHomeChallenge.Infrastructure/TakeHomeChallenge.Infrastructure.csproj src/TakeHomeChallenge.Infrastructure/
COPY src/TakeHomeChallenge.API/TakeHomeChallenge.API.csproj src/TakeHomeChallenge.API/

RUN dotnet restore src/TakeHomeChallenge.API/TakeHomeChallenge.API.csproj

# Copy everything else and publish
COPY src/ src/
RUN dotnet publish src/TakeHomeChallenge.API/TakeHomeChallenge.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Heroku dynamically assigns PORT
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TakeHomeChallenge.API.dll
