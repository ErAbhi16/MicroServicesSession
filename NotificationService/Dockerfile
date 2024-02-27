FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotificationService.csproj", "NotificationService/"]
RUN dotnet restore "NotificationService/NotificationService.csproj"
WORKDIR "/src/NotificationService"
COPY . .
RUN dotnet build "NotificationService.csproj" -c %BUILD_CONFIGURATION% -o /app/build
 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
 
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]