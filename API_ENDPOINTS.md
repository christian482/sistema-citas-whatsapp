# 🏥 API ENDPOINTS - Sistema de Citas WhatsApp

## 📍 Base URL: `http://localhost:5000/api`

---

## 🏢 BUSINESSES (Negocios)

### 📋 Listar todos los negocios
- **GET** `/businesses`
- **Respuesta:** Array de negocios con sus servicios

### 🔍 Obtener negocio por ID
- **GET** `/businesses/{id}`
- **Respuesta:** Negocio específico con servicios

### ➕ Crear negocio
- **POST** `/businesses`
- **Body:**
```json
{
  "name": "string (requerido)",
  "workingHours": "string (requerido)",
  "workingDays": "string (requerido)"
}
```

### ✏️ Actualizar negocio
- **PUT** `/businesses/{id}`
- **Body:**
```json
{
  "id": number,
  "name": "string (requerido)",
  "workingHours": "string (requerido)",
  "workingDays": "string (requerido)"
}
```

### 🗑️ Eliminar negocio
- **DELETE** `/businesses/{id}`
- **Validación:** No se puede eliminar si tiene servicios asociados

---

## 👨‍⚕️ DOCTORS (Doctores)

### 📋 Listar todos los doctores
- **GET** `/doctors`
- **Respuesta:** Array de doctores con sus citas

### 🔍 Obtener doctor por ID
- **GET** `/doctors/{id}`
- **Respuesta:** Doctor específico con citas completas

### ➕ Crear doctor
- **POST** `/doctors`
- **Body:**
```json
{
  "name": "string (requerido)",
  "specialty": "string (requerido)"
}
```

### ✏️ Actualizar doctor
- **PUT** `/doctors/{id}`
- **Body:**
```json
{
  "id": number,
  "name": "string (requerido)",
  "specialty": "string (requerido)"
}
```

### 🗑️ Eliminar doctor
- **DELETE** `/doctors/{id}`
- **Validación:** No se puede eliminar si tiene citas activas futuras

---

## 👥 CLIENTS (Clientes)

### 📋 Listar todos los clientes
- **GET** `/clients`
- **Respuesta:** Array de clientes con sus citas

### 🔍 Obtener cliente por ID
- **GET** `/clients/{id}`
- **Respuesta:** Cliente específico con citas completas

### ➕ Crear cliente
- **POST** `/clients`
- **Body:**
```json
{
  "name": "string (requerido)",
  "phone": "string (requerido)",
  "email": "string (requerido)"
}
```

### ✏️ Actualizar cliente
- **PUT** `/clients/{id}`
- **Body:**
```json
{
  "id": number,
  "name": "string (requerido)",
  "phone": "string (requerido)",
  "email": "string (requerido)"
}
```

### 🗑️ Eliminar cliente
- **DELETE** `/clients/{id}`
- **Validación:** No se puede eliminar si tiene citas activas futuras

---

## 🛠️ SERVICES (Servicios)

### 📋 Listar todos los servicios
- **GET** `/services`
- **Respuesta:** Array de servicios con información del negocio

### 🔍 Obtener servicio por ID
- **GET** `/services/{id}`
- **Respuesta:** Servicio específico con información del negocio

### 🏢 Obtener servicios por negocio
- **GET** `/services/business/{businessId}`
- **Respuesta:** Array de servicios de un negocio específico

### ➕ Crear servicio
- **POST** `/services`
- **Body:**
```json
{
  "name": "string (requerido)",
  "businessId": number (requerido)
}
```

### ✏️ Actualizar servicio
- **PUT** `/services/{id}`
- **Body:**
```json
{
  "id": number,
  "name": "string (requerido)",
  "businessId": number (requerido)
}
```

### 🗑️ Eliminar servicio
- **DELETE** `/services/{id}`
- **Validación:** No se puede eliminar si tiene citas activas asociadas

---

## 📅 APPOINTMENTS (Citas)

### 📋 Listar todas las citas
- **GET** `/appointments`
- **Respuesta:** Array de citas con información completa (cliente, doctor, servicio, negocio)

### 🔍 Obtener cita por ID
- **GET** `/appointments/{id}`
- **Respuesta:** Cita específica con información completa

### 👥 Obtener citas por cliente
- **GET** `/appointments/client/{clientId}`
- **Respuesta:** Array de citas del cliente

### 👨‍⚕️ Obtener citas por doctor
- **GET** `/appointments/doctor/{doctorId}`
- **Respuesta:** Array de citas del doctor

### 🏢 Obtener citas por negocio
- **GET** `/appointments/business/{businessId}`
- **Respuesta:** Array de citas del negocio

### 📅 Obtener citas por fecha
- **GET** `/appointments/date/{fecha}`
- **Formato fecha:** `YYYY-MM-DD`
- **Respuesta:** Array de citas de la fecha específica

### ➕ Crear cita
- **POST** `/appointments`
- **Body:**
```json
{
  "date": "2024-12-01T10:00:00Z",
  "clientId": number (requerido),
  "serviceId": number (requerido),
  "doctorId": number (requerido),
  "businessId": number (requerido)
}
```
- **Validaciones:** 
  - Fecha debe ser futura
  - No puede haber conflictos de horario para el doctor

### ✏️ Actualizar cita
- **PUT** `/appointments/{id}`
- **Body:**
```json
{
  "id": number,
  "date": "2024-12-01T10:00:00Z",
  "clientId": number (requerido),
  "serviceId": number (requerido),
  "doctorId": number (requerido),
  "businessId": number (requerido)
}
```
- **Validaciones:** 
  - No se puede modificar cita cancelada
  - Fecha debe ser futura
  - No puede haber conflictos de horario

### ❌ Cancelar cita
- **DELETE** `/appointments/{id}`
- **Acción:** Marca la cita como cancelada (isCancelled = true)

### 🔄 Reactivar cita
- **POST** `/appointments/{id}/reactivate`
- **Acción:** Reactiva una cita cancelada
- **Validaciones:**
  - La cita debe estar cancelada
  - La fecha debe ser futura
  - No puede haber conflictos de horario

---

## 🏥 HEALTH (Diagnóstico)

### ✅ Estado del sistema
- **GET** `/health`
- **Respuesta:** Estado básico de la aplicación y BD

### 📊 Diagnóstico detallado de BD
- **GET** `/health/database`
- **Respuesta:** Estadísticas completas y datos de ejemplo

---

## 🔧 CONFIGURACIÓN CORS

El backend está configurado para permitir:
- ✅ **Todos los orígenes** (incluyendo localhost:4200)
- ✅ **Todos los métodos** (GET, POST, PUT, DELETE)
- ✅ **Todos los headers**

---

## 📝 NOTAS IMPORTANTES

### 🔒 Validaciones Implementadas
- **Negocios:** Campos requeridos
- **Doctores:** Nombre único, campos requeridos
- **Clientes:** Email y teléfono únicos, campos requeridos
- **Servicios:** Nombre único por negocio
- **Citas:** Validación de fechas futuras, conflictos de horario

### 🚨 Restricciones de Eliminación
- **Negocios:** No se puede eliminar si tiene servicios
- **Doctores:** No se puede eliminar si tiene citas activas futuras
- **Clientes:** No se puede eliminar si tiene citas activas futuras
- **Servicios:** No se puede eliminar si tiene citas activas

### 🗄️ Base de Datos
- **Motor:** MySQL (XAMPP local)
- **Puerto:** 3306
- **BD:** sistema_citas_whatsapp
- **JSON:** Configurado para manejar ciclos de referencia

### 🚀 Estado del Backend
- **URL:** http://localhost:5000
- **Estado:** ✅ ACTIVO
- **Todos los CRUDs:** ✅ COMPLETOS
- **Listo para Angular:** ✅ SÍ

---

## 🎯 Para tu Frontend Angular

Tu aplicación en `http://localhost:4200/dashboard` puede consumir todos estos endpoints directamente. Asegúrate de:

1. **Configurar HttpClient** en Angular
2. **Usar las URLs completas** (http://localhost:5000/api/...)
3. **Manejar las respuestas JSON** según los formatos documentados
4. **Implementar manejo de errores** para las validaciones del backend

¡El backend está 100% listo para tu frontend Angular! 🎉
