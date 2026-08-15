FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["ScaleUp.API/ScaleUp.API.csproj", "ScaleUp.API/"]
COPY ["ScaleUp.Core/ScaleUp.Core.csproj", "ScaleUp.Core/"]
COPY ["ScaleUp.Infrastructure/ScaleUp.Infrastructure.csproj", "ScaleUp.Infrastructure/"]

RUN dotnet restore "ScaleUp.API/ScaleUp.API.csproj"

COPY . .

WORKDIR "/src/ScaleUp.API"

RUN dotnet publish "ScaleUp.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "ScaleUp.API.dll"]