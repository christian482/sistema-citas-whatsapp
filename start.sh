#!/bin/bash
set -e

echo "=== Sistema de Citas WhatsApp - Iniciando ==="
echo "Fecha: $(date)"
echo "Usuario: $(whoami)"
echo "Directorio: $(pwd)"

# Configurar puerto
PORT=${PORT:-5000}
echo "Puerto configurado: $PORT"

# Verificar archivos
echo "=== Verificando archivos ==="
if [ -f "citas.dll" ]; then
    echo "✓ citas.dll encontrado"
else
    echo "❌ citas.dll NO encontrado"
    ls -la
    exit 1
fi

# Verificar variables de entorno críticas
echo "=== Verificando configuración ==="
echo "ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-'not set'}"

if [ -z "$WHATSAPP_TOKEN" ]; then
    echo "ADVERTENCIA: WHATSAPP_TOKEN no está configurado"
else
    echo "✓ WHATSAPP_TOKEN configurado"
fi

if [ -z "$WHATSAPP_API_URL" ]; then
    echo "ADVERTENCIA: WHATSAPP_API_URL no está configurado"
else
    echo "✓ WHATSAPP_API_URL configurado"
fi

echo "=== Iniciando aplicación .NET ==="
echo "Comando: dotnet citas.dll --urls http://0.0.0.0:$PORT"

exec dotnet citas.dll --urls "http://0.0.0.0:$PORT"
