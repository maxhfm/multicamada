# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do código
COPY . ./

# Publica a aplicação
RUN dotnet publish -c Release -o out

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copia build final
COPY --from=build /app/out ./

# Porta do container
EXPOSE 8080

# Variável importante pro ASP.NET
ENV ASPNETCORE_URLS=http://+:8080

# Comando de execução
ENTRYPOINT ["dotnet", "Backend.dll"]