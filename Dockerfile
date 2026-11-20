# 1. Imagen base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV PORT=8080
EXPOSE 8080

# 2. Imagen de SDK y compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el .csproj respetando la subcarpeta CompraCerca.API
COPY ["CompraCerca.API/CompraCerca.API.csproj", "CompraCerca.API/"]
RUN dotnet restore "CompraCerca.API/CompraCerca.API.csproj"

# Copiar todo el código y compilar
COPY . .
WORKDIR "/src/CompraCerca.API"
RUN dotnet build "CompraCerca.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompraCerca.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "exec dotnet CompraCerca.API.dll --urls http://0.0.0.0:${PORT}"]