# Dockerfile con copia completa y verificación
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar TODO el contenido de publish recursivamente
COPY publish/ .

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Comando con debugging completo
CMD echo "=== INICIANDO APLICACION ===" && \
    echo "Puerto: $PORT" && \
    echo "Directorio actual: $(pwd)" && \
    echo "=== LISTADO COMPLETO DE ARCHIVOS ===" && \
    ls -laR && \
    echo "=== BUSCANDO ARCHIVOS .dll ===" && \
    find . -name "*.dll" -type f && \
    echo "=== VERIFICANDO citas.dll ===" && \
    if [ -f "./citas.dll" ]; then \
        echo "✓ citas.dll encontrado en raíz, ejecutando..." && \
        dotnet ./citas.dll --urls "http://0.0.0.0:${PORT:-5000}"; \
    elif [ -f "citas.dll" ]; then \
        echo "✓ citas.dll encontrado, ejecutando..." && \
        dotnet citas.dll --urls "http://0.0.0.0:${PORT:-5000}"; \
    else \
        echo "❌ citas.dll NO encontrado en ninguna ubicación" && \
        echo "Intentando ejecutar desde cualquier ubicación encontrada..." && \
        CITAS_DLL=$(find . -name "citas.dll" -type f | head -1) && \
        if [ -n "$CITAS_DLL" ]; then \
            echo "Encontrado en: $CITAS_DLL" && \
            dotnet "$CITAS_DLL" --urls "http://0.0.0.0:${PORT:-5000}"; \
        else \
            echo "ERROR: No se pudo encontrar citas.dll en ninguna parte" && \
            exit 1; \
        fi; \
    fi
