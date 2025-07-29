# Zie https://aka.ms/customizecontainer voor instructies over het aanpassen van deze container.

# ────────────────────────────────────────────────────────────────
# 1) Basis-runtime image met .NET en MySQL client-tools
# ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root

# Installeer MySQL client (inclusief mysqldump en mysql)
RUN apt-get update \
 && apt-get install -y --no-install-recommends default-mysql-client \
 && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 80
EXPOSE 443

# ────────────────────────────────────────────────────────────────
# 2) SDK-image met Node.js voor build van de API (indien nodig)
# ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS with-node

# Node.js installatie
RUN apt-get update \
 && apt-get install -y curl \
 && curl -sL https://deb.nodesource.com/setup_20.x | bash - \
 && apt-get install -y nodejs \
 && rm -rf /var/lib/apt/lists/*

# ────────────────────────────────────────────────────────────────
# 3) Build-stap voor Jogging.Api
# ────────────────────────────────────────────────────────────────
FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Alleen projectbestand kopiëren voor snellere restore
COPY ["Jogging.Api/Jogging.Api.csproj", "Jogging.Api/"]
RUN dotnet restore "Jogging.Api/Jogging.Api.csproj"

# Rest van de code kopiëren en bouwen
COPY . .
WORKDIR "/src/Jogging.Api"
RUN dotnet build "Jogging.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ────────────────────────────────────────────────────────────────
# 4) Publish-stap
# ────────────────────────────────────────────────────────────────
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Jogging.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ────────────────────────────────────────────────────────────────
# 5) Final image samenstellen
# ────────────────────────────────────────────────────────────────
FROM base AS final

WORKDIR /app

# Gepubliceerde output van vorige stap kopiëren
COPY --from=publish /app/publish .

# Application entrypoint
ENTRYPOINT ["dotnet", "Jogging.Api.dll"]
