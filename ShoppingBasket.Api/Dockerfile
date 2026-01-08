# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ShoppingBasket.Api/ShoppingBasket.Api.csproj", "ShoppingBasket.Api/"]
RUN dotnet restore "ShoppingBasket.Api/ShoppingBasket.Api.csproj"

COPY . .
WORKDIR "/src/ShoppingBasket.Api"
RUN dotnet build "ShoppingBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ShoppingBasket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingBasket.Api.dll"]