# Conexa Movies API

[![CI](https://github.com/nicolasundiano/Conexa/actions/workflows/ci.yml/badge.svg)](https://github.com/nicolasundiano/Conexa/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)

API de gestión de películas construida sobre **.NET 10**, que consume la [API pública de Star Wars (SWAPI)](https://www.swapi.tech/). Autenticación con JWT, autorización por roles, sincronización con SWAPI y documentación con Swagger.

## 🌐 Demo en vivo

**Swagger:** https://conexa-production-f745.up.railway.app/swagger

Desplegada en Railway con base de datos PostgreSQL en Neon.

## 🚀 Cómo ejecutar

### Opción 1 — Docker Compose (recomendado)

Un solo comando levanta la API + una base PostgreSQL, con migraciones y datos de prueba aplicados automáticamente:

```bash
docker compose up --build
```

Swagger disponible en **http://localhost:8080/swagger**. No requiere instalar el SDK de .NET ni PostgreSQL.

### Opción 2 — Local con el SDK de .NET

Requiere el **SDK de .NET 10** y una base PostgreSQL accesible. Configurar la cadena de conexión y la clave JWT (por ejemplo con `dotnet user-secrets` o variables de entorno):

```bash
cd src/Conexa.Api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=conexadb;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Key" "una-clave-secreta-de-al-menos-32-caracteres"
dotnet run
```

La aplicación aplica las migraciones y siembra los usuarios de prueba al arrancar.

> **Nota:** la base de datos arranca **sin películas**. Para importarlas desde SWAPI, iniciar sesión como Admin y ejecutar `POST /api/movies/sync` (luego, por la regla de acceso, el detalle de una película se consulta con el usuario Regular).

## 🔑 Credenciales de prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| Administrador | `admin@conexa.test` | `Admin123!` |
| Usuario Regular | `user@conexa.test` | `User123!` |

**Cómo autenticarse en Swagger:** ejecutar `POST /api/auth/login` con una de las credenciales, copiar el `accessToken` de la respuesta, hacer clic en **Authorize** (arriba a la derecha) y pegar el token.

## 📋 Endpoints

| Método | Ruta | Acceso |
|--------|------|--------|
| `POST` | `/api/auth/register` | Público |
| `POST` | `/api/auth/login` | Público |
| `GET` | `/api/movies` | Autenticado (cualquier rol) |
| `GET` | `/api/movies/{id}` | **Solo Regular** |
| `POST` | `/api/movies` | Solo Admin |
| `PUT` | `/api/movies/{id}` | Solo Admin |
| `DELETE` | `/api/movies/{id}` | Solo Admin |
| `POST` | `/api/movies/sync` | Solo Admin |

El endpoint `GET /api/movies` admite paginación vía query params: `?page=1&pageSize=10` (tamaño máximo 100).

## 🏗️ Arquitectura

El proyecto sigue **Clean Architecture** con separación estricta de responsabilidades y dependencias apuntando siempre hacia el dominio:

```
src/
├── Conexa.Domain          # Entidades, value objects, invariantes de negocio. Sin dependencias.
├── Conexa.Application      # Casos de uso (CQRS con MediatR), interfaces (puertos), validación.
├── Conexa.Infrastructure   # EF Core + PostgreSQL, JWT, cliente SWAPI, hashing.
└── Conexa.Api              # Controllers, middleware, composición y configuración.

tests/
├── Conexa.UnitTests        # Dominio, handlers, validators, paginación (con mocks).
└── Conexa.IntegrationTests # Endpoints y autorización por rol (WebApplicationFactory + Testcontainers).
```

## 🧠 Decisiones de diseño

- **CQRS con MediatR** — cada caso de uso es un `Command`/`Query` con su handler. La validación (FluentValidation) y el logging se resuelven de forma transversal en el pipeline de MediatR —un *behavior* de validación y un *pre-processor* de logging—, sin ensuciar los handlers.
- **DDD táctico** — las entidades son ricas en comportamiento (setters privados, *factory methods*, invariantes) en lugar de meras bolsas de propiedades. El value object `MovieDetails` centraliza la validación de los datos de una película.
- **`IApplicationDbContext` en vez de repositorios** — el `DbContext` de EF Core ya es un *Unit of Work* + repositorio; una capa de repositorios por encima sería redundante. Los handlers consultan a través de una interfaz que mantiene la Application desacoplada de EF.
- **Excepciones tipadas + handler global** — los errores esperados (404, 409, 401, 502) se modelan como excepciones que un `IExceptionHandler` global traduce a respuestas **RFC 7807** (`application/problem+json`). Se eligió sobre el *Result pattern* porque estos errores son terminales para la request y su mapeo a HTTP se centraliza en un único lugar.
- **Autenticación desacoplada de la implementación** — `IPasswordHasher` e `ITokenService` son puertos en la Application. Se reutiliza el `PasswordHasher<T>` de ASP.NET Core Identity (hashing PBKDF2 probado) **sin** adoptar el framework Identity completo, que traería 7 tablas y funcionalidad innecesaria para el alcance.
- **Seguridad en el login** — mensaje de error genérico y verificación de hash *dummy* ante un email inexistente, para igualar los tiempos de respuesta y evitar la enumeración de cuentas por *timing*.
- **Resiliencia hacia SWAPI** — el cliente HTTP es un *typed client* (`IHttpClientFactory`) con reintentos, *circuit breaker* y *timeouts* (`Microsoft.Extensions.Http.Resilience`). Cualquier fallo del servicio externo se traduce en un `502`, nunca en un `500` sin controlar.
- **Sincronización idempotente y tolerante** — el sync identifica las películas por su URL de SWAPI (upsert), por lo que re-ejecutarlo no duplica datos. Un registro externo malformado se omite y se registra, sin abortar toda la sincronización.
- **PostgreSQL con convención `snake_case`** — nombres de tablas y columnas en `snake_case` (convención nativa de Postgres) mediante `EFCore.NamingConventions`. Fechas persistidas como `timestamptz`.
- **Auditoría transversal** — un `SaveChangesInterceptor` completa los campos `created_at/by` y `last_modified_at/by` automáticamente, sin que las entidades ni los handlers se ocupen de ello.
- **DTOs de request separados de los commands** — la capa de API expone sus propios contratos, evitando *over-posting* y desacoplando el contrato HTTP de los tipos internos de la Application.

## 🧪 Testing

**60 tests** (48 unitarios + 12 de integración), ejecutados automáticamente en cada push mediante GitHub Actions.

```bash
dotnet test                          # todos
dotnet test tests/Conexa.UnitTests   # solo unitarios (no requieren Docker)
```

- **Unitarios** — invariantes del dominio, lógica de los handlers (con NSubstitute), validators y paginación.
- **Integración** — endpoints y **restricción de acceso por rol** (401/403), sobre una PostgreSQL real efímera levantada con **Testcontainers** (requiere Docker).

## 🛠️ Stack

.NET 10 · ASP.NET Core · Entity Framework Core · PostgreSQL · MediatR · FluentValidation · JWT · Swagger (Swashbuckle) · xUnit · NSubstitute · Testcontainers · Docker · GitHub Actions

## 🔭 Posibles mejoras

- *Refresh tokens* y expiración configurable por cliente.
- *Rate limiting* y CORS según el consumidor.
- Migraciones como paso de despliegue separado (en lugar de al arranque) para escenarios multi-instancia.
- Modelo extensible a **series** (mencionadas en el objetivo del ejercicio): la estructura actual (`MovieDetails`) permite incorporarlas sin refactor.
