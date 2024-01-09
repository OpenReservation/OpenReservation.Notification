FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OpenReservation.Notification/OpenReservation.Notification.csproj", "OpenReservation.Notification/"]
RUN dotnet restore "OpenReservation.Notification/OpenReservation.Notification.csproj"
COPY . .
WORKDIR "/src/OpenReservation.Notification"
RUN dotnet build "OpenReservation.Notification.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OpenReservation.Notification.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenReservation.Notification.dll"]
