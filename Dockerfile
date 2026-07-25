FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HailowApiGateway.csproj .
RUN dotnet restore HailowApiGateway.csproj

COPY . .
RUN dotnet publish HailowApiGateway.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HailowApiGateway.dll"]
