# Dockerfile simplificado sin script externo
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar archivos compilados
COPY publish/ ./

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Comando integrado con debugging
CMD echo "=== INICIANDO APLICACION ===" && \
    echo "Puerto: $PORT" && \
    echo "Directorio: $(pwd)" && \
    echo "Archivos disponibles:" && \
    ls -la && \
    echo "=== EJECUTANDO DOTNET ===" && \
    dotnet citas.dll --urls "http://0.0.0.0:${PORT:-5000}"
