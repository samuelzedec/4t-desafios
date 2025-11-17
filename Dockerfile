FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /build

RUN apk add --no-cache bash dos2unix
COPY Directory.Build.props Directory.Packages.props 4t-desafios.slnx ./

COPY src/Health.Api/Health.Api.csproj ./src/Health.Api/
COPY src/Health.Application/Health.Application.csproj ./src/Health.Application/
COPY src/Health.Infrastructure/Health.Infrastructure.csproj ./src/Health.Infrastructure/
COPY src/Health.Domain/Health.Domain.csproj ./src/Health.Domain/

COPY tests/Health.Application.Tests/Health.Application.Tests.csproj ./tests/Health.Application.Tests/
COPY tests/Health.Domain.Tests/Health.Domain.Tests.csproj ./tests/Health.Domain.Tests/
RUN dotnet restore 4t-desafios.slnx

COPY src/ ./src/
COPY tests/ ./tests/
RUN dotnet build 4t-desafios.slnx -c Release --no-restore

FROM build AS tests
WORKDIR /build

RUN dotnet test 4t-desafios.slnx \
    -c Release \
    --no-build \
    --no-restore \
    --logger "console;verbosity=detailed"

FROM build AS publish
WORKDIR /build

RUN dotnet publish src/Health.Api/Health.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    --no-build

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser && \
    apk add --no-cache bash

ENV ConnectionStrings__PostgresConnection=""

COPY --from=publish /app/publish .
RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080
ENTRYPOINT ["dotnet", "Health.Api.dll"]