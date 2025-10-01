# Dockerfile con verificación de archivos
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar archivos compilados de manera más específica
COPY publish/*.dll ./
COPY publish/*.json ./
COPY publish/*.config ./
COPY publish/runtimes ./runtimes/

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Comando con verificación mejorada
CMD echo "=== INICIANDO APLICACION ===" && \
    echo "Puerto: $PORT" && \
    echo "Directorio: $(pwd)" && \
    echo "Archivos .dll disponibles:" && \
    ls -la *.dll || echo "No hay archivos .dll" && \
    echo "Todos los archivos:" && \
    ls -la && \
    echo "=== VERIFICANDO citas.dll ===" && \
    if [ -f "citas.dll" ]; then \
        echo "✓ citas.dll encontrado, ejecutando..." && \
        dotnet citas.dll --urls "http://0.0.0.0:${PORT:-5000}"; \
    else \
        echo "❌ citas.dll NO encontrado" && \
        echo "Contenido del directorio:" && \
        find . -name "*.dll" && \
        exit 1; \
    fi
