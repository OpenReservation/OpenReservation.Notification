FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
WORKDIR /src
COPY ["OpenReservation.Notification/OpenReservation.Notification.csproj", "OpenReservation.Notification/"]
RUN dotnet restore "OpenReservation.Notification/OpenReservation.Notification.csproj"
COPY . .
WORKDIR "/src/OpenReservation.Notification"
RUN dotnet publish "OpenReservation.Notification.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenReservation.Notification.dll"]
