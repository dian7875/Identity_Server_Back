# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Identity Server Backend/Identity Server Backend.csproj", "Identity Server Backend/"]
COPY ["Identity.Application/Identity.Application.csproj", "Identity.Application/"]
COPY ["Identity.Domain/Identity.Domain.csproj", "Identity.Domain/"]
COPY ["Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "Identity Server Backend/Identity Server Backend.csproj"

# Copiar el resto del código fuente y compilar
COPY . .
WORKDIR "/app/Identity Server Backend"
RUN dotnet publish -c Release -o /app/out

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto esperado por Railway
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 7222

# Comando de inicio
ENTRYPOINT ["dotnet", "Identity Server Backend.dll"]
