# README del Backend

## Descripción del Proyecto
El backend de este proyecto ha sido desarrollado en **.NET** utilizando **ASP.NET Core**. Proporciona un conjunto de APIs RESTful para la gestión de suscripciones y planes de telefonía móvil. La aplicación está diseñada para integrarse con el frontend de Svelte y se ha desplegado en **Azure App Service** para garantizar escalabilidad y disponibilidad.

## Tecnologías Utilizadas
- **Framework**: ASP.NET Core 6.0
- **Base de Datos**: SQL Server (despliegue local y Azure SQL Database)
- **Autenticación**: JWT (JSON Web Tokens)
- **IDE**: Visual Studio 2022
- **Alojamiento**: Azure App Service
- **Control de Versiones**: Git (GitHub)

## Arquitectura
El backend sigue una arquitectura limpia, basada en separación de responsabilidades:

- **Controllers**: Manejan las solicitudes HTTP entrantes y devuelven las respuestas.
- **Services**: Contienen la lógica de negocio y se encargan de la interacción con los repositorios.
- **Repositories**: Se comunican con la base de datos para realizar operaciones CRUD.
- **Models**: Definen las entidades del dominio.

## Estructura del Proyecto
La estructura del proyecto está organizada de la siguiente manera:

- **Controllers**: `PlanController`, `SuscripcionController`, `UsuarioController`.
- **Services**: `PlanService`, `SuscripcionService`, `UsuarioService`.
- **Data**: Configuraciones de la base de datos y contexto de EF Core (`ApplicationDbContext`).
- **Models**: Entidades como `Plan`, `Suscripcion`, `Usuario`.
- **DTOs**: Objetos de transferencia de datos para estructurar las solicitudes y respuestas.

## Características Principales
- **Autenticación y Autorización**: Sistema de login basado en JWT para gestionar roles y permisos.
- **Gestor de Planes de Teléfono**: CRUD completo para crear, leer, actualizar y eliminar planes de teléfono.
- **Gestor de Suscripciones**: CRUD para gestionar suscripciones de usuarios.
- **Protección de Endpoints**: Middleware de autorización para restringir el acceso según el rol del usuario.

## Instalación y Configuración
Para ejecutar el backend localmente, siga los siguientes pasos:

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/yourusername/your-backend-repo.git
   ```

2. **Navegar al directorio del proyecto**:
   ```bash
   cd your-backend-repo
   ```

3. **Configurar la base de datos**:
   Actualice la cadena de conexión en `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TelefoniaMovilDb;Trusted_Connection=True;"
   }
   ```

4. **Restaurar paquetes NuGet**:
   ```bash
   dotnet restore
   ```

5. **Aplicar migraciones y actualizar la base de datos**:
   ```bash
   dotnet ef database update
   ```

6. **Ejecutar la aplicación**:
   ```bash
   dotnet run
   ```

## Despliegue
El backend está desplegado en **Azure App Service** para garantizar su disponibilidad. La aplicación se conecta a una base de datos en **Azure SQL Database** para el almacenamiento de información.

### Pasos para el Despliegue en Azure:
1. **Publicar el proyecto desde Visual Studio**:
   - Seleccione la opción de publicar y configure el perfil de publicación en Azure App Service.
2. **Configurar la base de datos en Azure SQL Database**.
3. **Actualizar las variables de entorno en Azure** para incluir la cadena de conexión a la base de datos y la clave secreta de JWT.

## Buenas Prácticas Implementadas
- **Inyección de Dependencias**: Uso de servicios registrados en `Startup.cs` para fomentar la escalabilidad y facilidad de prueba.
- **DTOs**: Para la transferencia de datos, manteniendo la seguridad de los modelos de datos.
- **Middleware Personalizado**: Implementación de middleware para el manejo de errores y la protección de endpoints.
- **Seguridad**: Uso de JWT para la autenticación y autorización, protegiendo datos sensibles.

## Mejoras Futuras
- **Implementación de Logs**: Integrar un sistema de logging con `Serilog` o `NLog`.
- **Documentación de API**: Agregar documentación con **Swagger**.
- **Pruebas Unitarias y de Integración**: Implementar más pruebas automatizadas para mejorar la calidad del código.
- **Monitoreo**: Configurar **Application Insights** para el seguimiento del rendimiento en producción.

## Contacto y Soporte
Para cualquier problema o consulta, por favor contacte:
- **Nombre**: Carlos Esteban Larco
- **Correo Electrónico**: carlos.larco.escobar@udla.edu.ec

