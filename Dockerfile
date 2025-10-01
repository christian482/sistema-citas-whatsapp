# Dockerfile con script de inicio para debugging error 145
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar archivos
COPY publish/ ./
COPY start.sh ./

# Dar permisos al script
RUN chmod +x start.sh

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Usar script de inicio
CMD ["./start.sh"]
