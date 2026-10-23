FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OpenEduSyncShare.Api/OpenEduSyncShare.Api.csproj", "OpenEduSyncShare.Api/"]
COPY ["OpenEduSyncShare.Application/OpenEduSyncShare.Application.csproj", "OpenEduSyncShare.Application/"]
COPY ["OpenEduSyncShare.Domain/OpenEduSyncShare.Domain.csproj", "OpenEduSyncShare.Domain/"]
COPY ["OpenEduSyncShare.Infrastructure/OpenEduSyncShare.Infrastructure.csproj", "OpenEduSyncShare.Infrastructure/"]
RUN dotnet restore "OpenEduSyncShare.Api/OpenEduSyncShare.Api.csproj"

COPY . .
RUN dotnet publish "OpenEduSyncShare.Api/OpenEduSyncShare.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OpenEduSyncShare.Api.dll"]
