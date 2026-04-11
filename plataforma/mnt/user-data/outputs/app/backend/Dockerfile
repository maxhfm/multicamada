# ── Build ──────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Backend.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# ── Runtime ────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Variáveis sobrescritas pelo Kubernetes via Secret/ConfigMap
ENV DB_HOST=mysql-service \
    DB_NAME=appdb \
    DB_USER=appuser \
    DB_PASSWORD=changeme

# Substitui as variáveis de ambiente na connection string em runtime
ENV ConnectionStrings__Default="Server=${DB_HOST};Port=3306;Database=${DB_NAME};User=${DB_USER};Password=${DB_PASSWORD};"

EXPOSE 8080
ENTRYPOINT ["dotnet", "Backend.dll"]
