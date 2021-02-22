FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "FinanceMonitor.sln" "FinanceMonitor.sln"

COPY "FinanceMonitor.Identity/FinanceMonitor.Identity.csproj" "FinanceMonitor.Identity/FinanceMonitor.Identity.csproj"
COPY "FinanceMonitor.Messages/FinanceMonitor.Messages.csproj" "FinanceMonitor.Messages/FinanceMonitor.Messages.csproj"
COPY "FinanceMonitor.Api/FinanceMonitor.Api.csproj" "FinanceMonitor.Api/FinanceMonitor.Api.csproj"
COPY "FinanceMonitor.DAL/FinanceMonitor.DAL.csproj" "FinanceMonitor.DAL/FinanceMonitor.DAL.csproj"


# COPY "docker-compose.dcproj" "docker-compose.dcproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "FinanceMonitor.sln"

COPY . .
WORKDIR /src/FinanceMonitor.Api
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FinanceMonitor.Api.dll"]
