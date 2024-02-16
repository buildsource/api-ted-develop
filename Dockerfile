FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ["./src/TED.API/NuGet.config", "."]
COPY ["./src/TED.API/TED.API.csproj", "./"]
RUN dotnet restore "TED.API.csproj"

COPY ["./src/TED.API", "./TED.API"]
RUN dotnet build "TED.API/TED.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TED.API/TED.API.csproj" -c Release -o /app/publish

FROM FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "TED.API.dll"]
