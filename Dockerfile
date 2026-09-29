# 1. Imagen base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Imagen de SDK y compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj respetando la subcarpeta CompraCerca.API
COPY ["CompraCerca.API/CompraCerca.API.csproj", "CompraCerca.API/"]
RUN dotnet restore "CompraCerca.API/CompraCerca.API.csproj"

# Copiar todo el código y compilar
COPY . .
WORKDIR "/src/CompraCerca.API"
RUN dotnet build "CompraCerca.API.csproj" -c Release -o /app/build

# 3. Publicación
FROM build AS publish
RUN dotnet publish "CompraCerca.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompraCerca.API.dll"]