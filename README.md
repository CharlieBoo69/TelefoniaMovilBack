# Telefonía Móvil Backend

Este es el backend del sistema de gestión de planes y suscripciones de telefonía móvil. Proporciona una API RESTful que permite a los administradores gestionar planes, usuarios y suscripciones, además de funcionalidades avanzadas como generación de reportes y recomendaciones personalizadas para los usuarios.

---

## 🛠 Tecnologías Utilizadas

- **Framework**: ASP.NET Core
- **Base de Datos**: SQL Server (hospedado en Azure)
- **Autenticación**: JWT y Cookies
- **ORM**: Entity Framework Core
- **Lenguaje**: C#

---

## 🚀 Características

### Funcionalidades Principales:
1. **Gestión de Usuarios**:
   - Crear, editar y eliminar usuarios desde el panel de administración.
2. **Gestión de Planes**:
   - Crear, editar, eliminar y listar planes de telefonía.
   - Filtrar planes por operadora.
3. **Gestión de Suscripciones**:
   - Crear suscripciones asociadas a un usuario y un número de teléfono.
   - Evitar duplicados por número de teléfono.
4. **Recomendaciones de Planes**:
   - Basadas en las preferencias de costo, datos, minutos y operadora del usuario.
5. **Reportes**:
   - Listado de usuarios con al menos dos suscripciones.
   - Identificación de planes repetidos (suscritos por el mismo usuario con diferentes números).

### Seguridad:
- Autenticación mediante JWT.
- Restricción de acceso según roles (`admin` y `user`).
- Gestión de sesiones con cookies seguras (`HttpOnly`, `Secure`, `SameSite`).

---

## 📂 Estructura del Proyecto

├── Controllers │ ├── AuthController.cs # Controlador de autenticación (login, logout) │ ├── PlanApiController.cs # CRUD y recomendaciones de planes │ ├── SuscripcionController.cs # CRUD y reportes de suscripciones │ └── UsuarioController.cs # CRUD de usuarios ├── Data │ ├── ApplicationDbContext.cs # Contexto de la base de datos ├── Models │ ├── Plan.cs # Modelo de planes │ ├── Suscripcion.cs # Modelo de suscripciones │ ├── Usuario.cs # Modelo de usuarios │ └── UserPreferences.cs # Preferencias para recomendaciones ├── Program.cs # Configuración principal del backend └── appsettings.json # Configuración de la base de datos y JWT



---

## 🌐 Endpoints Principales

### Autenticación
- **`POST /api/Auth/login`**: Iniciar sesión y obtener un token JWT.
- **`POST /api/Auth/logout`**: Cerrar sesión y eliminar cookies.

### Usuarios
- **`GET /api/Usuario`**: Listar usuarios.
- **`POST /api/Usuario`**: Crear usuario (solo admin).
- **`PUT /api/Usuario/{id}`**: Editar usuario (solo admin).
- **`DELETE /api/Usuario/{id}`**: Eliminar usuario (solo admin).

### Planes
- **`GET /api/PlanApi`**: Listar planes.
- **`GET /api/PlanApi/ByOperadora/{operadora}`**: Filtrar planes por operadora.
- **`POST /api/PlanApi`**: Crear un nuevo plan (solo admin).
- **`PUT /api/PlanApi/{id}`**: Editar un plan existente (solo admin).
- **`DELETE /api/PlanApi/{id}`**: Eliminar un plan (solo admin).

### Suscripciones
- **`POST /api/Suscripcion/subscribe`**: Crear suscripción para un usuario autenticado.
- **`GET /api/Suscripcion/ReporteUsuarios`**: Generar un reporte de usuarios con suscripciones.

---

## ⚙️ Configuración

### Variables de Entorno
En el archivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:tu-servidor.database.windows.net;Database=tu-bd;User Id=tu-usuario;Password=tu-contraseña;"
  },
  "Jwt": {
    "Key": "tu_clave_secreta",
    "Issuer": "tu_emisor",
    "Audience": "tu_audiencia"
  }
}
```
### Instrucciones:
1. Sustituye **`tu-servidor`**, **`tu-bd`**, **`tu-usuario`**, **`tu_clave_secreta`**, etc., por los valores reales de tu proyecto.
2. Si necesitas agregar algo más específico o ejemplos adicionales, házmelo saber.
