# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FraudexPy.Api.csproj", "./"]
RUN dotnet restore "FraudexPy.Api.csproj"

COPY . .
RUN dotnet build "FraudexPy.Api.csproj" -c Release -o /app/build
RUN dotnet publish "FraudexPy.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FraudexPy.Api.dll"]
