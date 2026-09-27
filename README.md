# FraudexPy.Api (.NET 10 Web API)

API Backend para la plataforma de análisis y detección de fraudes **FraudexPy**, construida con tecnologías modernas y arquitectura limpia basada en CQRS.

## Características Técnicas

- **Framework:** .NET 10 (`net10.0`)
- **Base de Datos:** MySQL con Entity Framework Core (Pomelo)
- **Patrón:** CQRS desacoplado con **MediatR**
- **Autenticación:** JWT Bearer (JSON Web Tokens)
- **Almacenamiento de Objetos:** MinIO / S3 Object Storage
- **Documentación OpenAPI:** Swagger UI habilitado incondicionalmente para desarrollo y producción
- **Configuración:** Archivo único `appsettings.json` preparado para sobreescritura 100% mediante Variables de Entorno en CapRover / Docker

## Variables de Entorno (CapRover / Docker)

| Variable de Entorno | Descripción | Valor por defecto (Local) |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión MySQL | `Server=localhost;Port=3306;Database=fraudex_db;User=root;Password=root_password;` |
| `JwtSettings__SecretKey` | Clave secreta para firmar tokens JWT | Clave por defecto |
| `JwtSettings__Issuer` | Emisor del token JWT | `FraudexPy.Api` |
| `JwtSettings__Audience` | Audiencia del token JWT | `FraudexPy.Client` |
| `JwtSettings__ExpirationInMinutes` | Expiración en minutos | `1440` (24 horas) |
| `MinioSettings__Endpoint` | Host de MinIO / S3 | `localhost:9000` |
| `MinioSettings__AccessKey` | Access Key de MinIO | `minioadmin` |
| `MinioSettings__SecretKey` | Secret Key de MinIO | `minioadmin` |
| `MinioSettings__BucketName` | Nombre del bucket | `fraudex-documents` |
| `MinioSettings__UseSSL` | Usar SSL/HTTPS para MinIO | `false` |

## Endpoints Principales

- `POST /api/auth/register`: Registro de usuarios
- `POST /api/auth/login`: Autenticación y obtención de JWT
- `GET /api/auth/me`: Datos del usuario autenticado
- `GET /api/fraudreports`: Listado y filtrado de reportes de fraude
- `POST /api/fraudreports`: Creación de un nuevo reporte
- `POST /api/storage/upload`: Subida de archivos / evidencias a MinIO
- `GET /api/storage/download-url/{objectName}`: Obtención de URL prefirmada de descarga
- `GET /api/health`: Healthcheck del servicio
- `GET /swagger`: Interfaz interactiva de Swagger UI
