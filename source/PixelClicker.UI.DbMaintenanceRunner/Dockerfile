FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PixelClicker.UI.DbMaintenanceRunner/PixelClicker.UI.DbMaintenanceRunner.csproj", "PixelClicker.UI.DbMaintenanceRunner/"]
RUN dotnet restore "PixelClicker.UI.DbMaintenanceRunner/PixelClicker.UI.DbMaintenanceRunner.csproj"
COPY . .
WORKDIR "/src/PixelClicker.UI.DbMaintenanceRunner"
RUN dotnet build "PixelClicker.UI.DbMaintenanceRunner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PixelClicker.UI.DbMaintenanceRunner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelClicker.UI.DbMaintenanceRunner.dll"]
