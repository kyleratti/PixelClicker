FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG BUILD_CLIENT_APP=false
WORKDIR /src
COPY ["PixelClicker.UI.WebApi.csproj", "./"]
RUN dotnet restore "PixelClicker.UI.WebApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "PixelClicker.UI.WebApi.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build /p:BuildClientApp=$BUILD_CLIENT_APP

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PixelClicker.UI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelClicker.UI.WebApi.dll"]
