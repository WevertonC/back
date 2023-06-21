FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# Sem essa biblioteca, da erro no deploy por causa do fastreport
RUN apt-get update && apt-get install -y libgdiplus
# Biblioteca usada para ajeitar as fontes dos relatórios em pdf
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig

WORKDIR /app
# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /app
WORKDIR /src

# Sem essa biblioteca, da erro no deploy por causa do fastreport
RUN apt-get update && apt-get install -y libgdiplus
# Biblioteca usada para ajeitar as fontes dos relatórios em pdf
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt-get update; apt-get install -y ttf-mscorefonts-installer fontconfig

# Install production dependencies.
# Copy csproj and restore as distinct layers.
COPY ["WSolucaoWeb/WSolucaoWeb.csproj", "WSolucaoWeb/"]

COPY ["Persistencia/Persistencia.csproj", "Persistencia/"]

RUN dotnet restore "WSolucaoWeb/WSolucaoWeb.csproj"

# Copy local code to the container image.
COPY . ./
WORKDIR "/src/WSolucaoWeb/"
RUN dotnet build "WSolucaoWeb.csproj" -c Release -o /app/build

# Build a release artifact.
FROM build as publish
RUN dotnet publish "WSolucaoWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run the web service on container startup.
ENTRYPOINT ["dotnet", "WSolucaoWeb.dll"]