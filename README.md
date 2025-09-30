# Sistema de Citas con WhatsApp API

## Descripción
API REST desarrollada en .NET 8 para gestión de citas médicas con integración a WhatsApp Cloud API. Permite a los usuarios agendar, cancelar y consultar citas directamente desde WhatsApp.

## Características
- ✅ API REST completa para gestión de citas
- ✅ Integración con WhatsApp Cloud API
- ✅ Base de datos SQL Server con Entity Framework Core
- ✅ Recordatorios automáticos por WhatsApp
- ✅ Comandos por WhatsApp: "Agendar", "Cancelar", "Mis citas", "Horarios"
- ✅ Endpoints para negocios, médicos, clientes, servicios y citas

## Tecnologías
- .NET 8 Web API
- Entity Framework Core
- SQL Server
- WhatsApp Cloud API
- Background Services para recordatorios

## Instalación

### Prerrequisitos
- .NET 8 SDK
- SQL Server (local o remoto)
- Cuenta de WhatsApp Business API

### Configuración
1. Clona el repositorio:
```bash
git clone https://github.com/tu-usuario/sistema-citas-whatsapp.git
cd sistema-citas-whatsapp
```

2. Configura la base de datos en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CitasDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "WhatsApp": {
    "ApiUrl": "https://graph.facebook.com/v19.0/TU-NUMERO/messages",
    "Token": "TU-TOKEN-DE-WHATSAPP"
  }
}
```

3. Ejecuta las migraciones:
```bash
dotnet ef database update
```

4. Ejecuta el proyecto:
```bash
dotnet run
```

## Endpoints Principales

### API REST
- `GET/POST /api/business` - Gestión de negocios/consultorios
- `GET/POST /api/doctor` - Gestión de médicos
- `GET/POST /api/client` - Gestión de clientes/pacientes
- `GET/POST /api/service` - Gestión de servicios médicos
- `GET/POST/DELETE /api/appointment` - Gestión de citas

### WhatsApp Integration
- `POST /api/whatsappwebhook` - Recibe mensajes de WhatsApp
- `GET /api/whatsappwebhook` - Verificación del webhook
- `GET /api/whatsappwebhook/privacidad` - Aviso de privacidad

### Monitoreo
- `GET /api/health` - Estado del sistema
- `GET /api/health/ping` - Ping de conectividad

## Comandos de WhatsApp

Los usuarios pueden enviar estos comandos por WhatsApp:

- **Agendar cita:** `Agendar 2025-10-01 10:00 Dr. Juan`
- **Cancelar cita:** `Cancelar 2025-10-01 10:00`
- **Ver mis citas:** `Mis citas`
- **Ver horarios disponibles:** `Horarios Dr. Juan 2025-10-01`

## Estructura del Proyecto
```
├── Controllers/          # Controladores de la API
├── Models/              # Modelos de datos
├── Data/                # DbContext y configuración de EF
├── Services/            # Servicios (WhatsApp, Recordatorios)
├── Migrations/          # Migraciones de base de datos
└── Program.cs           # Configuración de la aplicación
```

## Configuración de WhatsApp

1. Configura tu webhook en el panel de Meta for Developers
2. URL del webhook: `https://tu-dominio.com/api/whatsappwebhook`
3. Token de verificación: configúralo en `WhatsAppWebhookController.cs`

## Despliegue

### Render.com (Recomendado para desarrollo)
El proyecto incluye configuración completa para Render.com:

1. Conecta tu repositorio GitHub a Render.com
2. Selecciona "Web Service" con "Docker Environment"
3. Configura las variables de entorno de WhatsApp
4. Ver [DEPLOY.md](DEPLOY.md) para instrucciones detalladas

### Docker
```bash
docker build -t sistema-citas .
docker run -p 8080:8080 sistema-citas
```

### Archivos de configuración incluidos:
- `Dockerfile` - Imagen Docker
- `render.yaml` - Configuración Render.com
- `appsettings.Production.json` - Configuración producción

## Licencia
[MIT License](LICENSE)

## Contribución
Las contribuciones son bienvenidas. Por favor, abre un issue antes de enviar un pull request.
