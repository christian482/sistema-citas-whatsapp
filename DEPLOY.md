# Despliegue en Render.com

## Pasos para desplegar en Render.com

### 1. Preparar el repositorio
✅ **Ya completado** - El repositorio ya tiene todos los archivos necesarios:
- `Dockerfile` - Configuración Docker
- `.dockerignore` - Optimización del contenedor
- `render.yaml` - Configuración de servicios
- `appsettings.Production.json` - Configuración de producción

### 2. Configurar en Render.com

1. **Crear cuenta en Render.com**
   - Ve a [https://render.com](https://render.com)
   - Regístrate o inicia sesión

2. **Conectar repositorio**
   - Clic en "New +" → "Web Service"
   - Conecta tu cuenta de GitHub
   - Selecciona el repositorio: `christian482/sistema-citas-whatsapp`

3. **Configurar el servicio web**
   - **Name**: `sistema-citas-whatsapp`
   - **Environment**: `Docker`
   - **Plan**: `Free` (para empezar)
   - **Dockerfile Path**: `./Dockerfile`

4. **Variables de entorno importantes**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   WHATSAPP_API_URL=https://graph.facebook.com/v19.0/TU_NUMERO_WHATSAPP/messages
   WHATSAPP_TOKEN=TU_TOKEN_DE_WHATSAPP
   ```

### 3. Configurar base de datos (Opcional)

Para usar PostgreSQL en Render.com:
1. Crear servicio PostgreSQL en Render
2. Usar la cadena de conexión proporcionada
3. Actualizar la variable de entorno `DATABASE_URL`

### 4. Para desarrollo local con SQL Server

El proyecto está configurado para usar:
- **SQL Server** en desarrollo local
- **PostgreSQL** en producción (Render.com)

### 5. Webhook de WhatsApp

Una vez desplegado, tu URL será:
```
https://tu-app-name.onrender.com/webhook/whatsapp
```

Configura esta URL en tu aplicación de WhatsApp Business API.

### 6. Endpoints disponibles

- `GET /health` - Estado del servicio
- `POST /webhook/whatsapp` - Webhook WhatsApp
- `GET /webhook/whatsapp` - Verificación webhook
- `GET /api/businesses` - Listar negocios
- `POST /api/businesses` - Crear negocio
- Y más endpoints para doctores, clientes, servicios y citas

### Notas importantes

- El plan gratuito de Render tiene limitaciones
- La aplicación se "duerme" después de 15 minutos de inactividad
- Para producción real, considera un plan pago

### Troubleshooting

Si tienes problemas:

#### Error 139 (Segfault)
- ✅ **Solucionado** - El Dockerfile ha sido optimizado
- Usar puerto dinámico (`$PORT`) en lugar de puerto fijo
- Script de inicio incluido para debug

#### Otros problemas comunes:
1. **Logs**: Revisa los logs en el dashboard de Render
2. **Variables de entorno**: Verifica que todas estén configuradas
3. **Token WhatsApp**: Asegúrate de que sea válido
4. **Webhook URL**: Confirma que esté correctamente configurada

#### Configuración recomendada en Render.com:
```
Environment: Docker
Build Command: (dejar vacío)
Start Command: (dejar vacío - usa Dockerfile)
```

#### Variables de entorno mínimas:
```
ASPNETCORE_ENVIRONMENT=Production
WHATSAPP_TOKEN=tu_token_real
WHATSAPP_API_URL=https://graph.facebook.com/v19.0/TU_NUMERO/messages
```
