# ─────────────────────────────────────────────────────────────
#  STAGE 1 — Build
# ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (layer caching — faster rebuilds)
# Adjust these paths if your folder names differ.
COPY ["srv.slots.api/srv.slots.api.csproj", "srv.slots.api/"]
COPY ["srv.slots.application/srv.slots.application.csproj", "srv.slots.application/"]
COPY ["srv.slots.infrastructure/srv.slots.infrastructure.csproj", "srv.slots.infrastructure/"]
COPY ["srv.slots.domain/srv.slots.domain.csproj", "srv.slots.domain/"]

# Restore dependencies
RUN dotnet restore "srv.slots.api/srv.slots.api.csproj"

# Copy the rest of the source
COPY . .

# Build and publish
WORKDIR "/src/srv.slots.api"
RUN dotnet publish "srv.slots.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ─────────────────────────────────────────────────────────────
#  STAGE 2 — Runtime (smaller image, no SDK)
# ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render provides the PORT env var; bind Kestrel to it.
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "srv.slots.api.dll"]
