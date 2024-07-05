FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG BUILD_CLIENT_APP=false
# Install node
RUN mkdir -p /etc/apt/keyrings && \
    apt-get update && \
    apt-get install -y gnupg && \
    curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg && \
    NODE_MAJOR=20 && \
    echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" > /etc/apt/sources.list.d/nodesource.list && \
    apt-get update && \
    apt-get install -y nodejs
WORKDIR /src
COPY ["PixelClicker.UI.WebApi.csproj", "./"]
RUN dotnet restore "PixelClicker.UI.WebApi.csproj"
COPY . .
WORKDIR "/src"
RUN ls -al ./ClientApp/
RUN dotnet build "PixelClicker.UI.WebApi.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build /p:BuildClientApp=$BUILD_CLIENT_APP

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PixelClicker.UI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PixelClicker.UI.WebApi.dll"]
