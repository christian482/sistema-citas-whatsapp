#!/bin/bash

echo "=== Sistema de Citas WhatsApp - Iniciando ==="
echo "Fecha: $(date)"
echo "Puerto: ${PORT:-8080}"
echo "Ambiente: ${ASPNETCORE_ENVIRONMENT:-Development}"

# Verificar variables de entorno críticas
echo "=== Verificando configuración ==="
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
exec dotnet citas.dll
