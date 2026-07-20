# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CarMarket.Api.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway/Render inyectan la variable PORT en runtime; Program.cs la lee
# y hace que Kestrel escuche ahí. Este EXPOSE es solo documentación.
EXPOSE 8080

# Volumen para persistir la base SQLite entre despliegues.
# En Render: Settings > Disks. En Railway: agregar un Volume y montarlo en /app/data.
VOLUME ["/app/data"]
ENV ConnectionStrings__Default="Data Source=/app/data/carmarket.db"

ENTRYPOINT ["dotnet", "CarMarket.Api.dll"]
