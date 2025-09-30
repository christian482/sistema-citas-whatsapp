# Usar imagen base de .NET 8 SDK para compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar código fuente y compilar
COPY . ./
RUN dotnet publish -c Release -o out --no-restore

# Imagen runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar archivos compilados
COPY --from=build /app/out .

# Copiar script de inicio
COPY start.sh .
RUN chmod +x start.sh

# Variables de entorno por defecto
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# El puerto será dinámico según Render.com
EXPOSE $PORT

# Usar script de inicio
CMD ["./start.sh"]
