FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderService.csproj", "OrderService/"]
RUN dotnet restore "OrderService/OrderService.csproj"
WORKDIR "/src/OrderService"
COPY . .
RUN dotnet build "OrderService.csproj" -c %BUILD_CONFIGURATION% -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]