FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./src/TED.API/NuGet.config", "."]
COPY ["./src/TED.API/TED.API.csproj", "./"]
RUN dotnet restore "TED.API.csproj"

COPY ["./src/TED.API", "./TED.API"]
RUN dotnet build "TED.API/TED.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TED.API/TED.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "TED.API.dll"]
