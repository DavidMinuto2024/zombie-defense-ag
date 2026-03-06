# Zombie Defense API

API REST para el cálculo y registro de estrategias óptimas de eliminación de zombis, utilizando programación dinámica (Unbounded Knapsack 2D) como algoritmo base.

## Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Requisitos previos](#requisitos-previos)
- [Configuración y ejecución](#configuración-y-ejecución)
- [Endpoints](#endpoints)
- [Autenticación](#autenticación)
- [Esquema de base de datos](#esquema-de-base-de-datos)
- [Estructura del proyecto](#estructura-del-proyecto)

---

## Descripción

Zombie Defense API calcula, dado un número de balas y segundos disponibles, la estrategia óptima para maximizar el puntaje de eliminación de zombis. Cada simulación ejecutada queda persistida en la base de datos, permitiendo consultar el historial ordenado por puntaje.

**Flujo principal:**

1. El cliente envía la cantidad de balas y segundos disponibles.
2. El API ejecuta el algoritmo DP 2D sobre los tipos de zombis registrados en la base de datos.
3. Devuelve la lista óptima de zombis a eliminar junto con el puntaje total.
4. La simulación se guarda automáticamente en el historial.

---

## Arquitectura

El proyecto sigue **Arquitectura Limpia** con capas desacopladas y el patrón **Hexagonal (Puertos y Adaptadores)**:

```
zombie-defense.Domain          ← Entidades y contratos (sin dependencias externas)
zombie-defense.Application     ← Casos de uso y DTOs
zombie-defense.Infraestructure ← Implementaciones EF Core / SQL Server
zombie-defense (API)           ← Controllers, Middleware, configuración ASP.NET Core
```

**Dependencias entre capas:**

```
API → Application → Domain
API → Infraestructure → Domain
```

---

## Tecnologías

| Componente | Tecnología |
|---|---|
| Framework | .NET 10 / ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Base de datos | SQL Server |
| Documentación API | OpenAPI + Scalar UI |
| Autenticación | API Key (middleware custom) |

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server en `localhost:1433` (o Docker)
- Usuario `sa` habilitado con contraseña configurada

**Opcional — SQL Server con Docker:**

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ZombieDefense2024!" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## Configuración y ejecución

### 1. Clonar el repositorio

```bash
git clone <url-del-repo>
cd zombie-defense
```

### 2. Configurar cadena de conexión

Editar `zombie-defense/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=zombie-defensedb;User Id=sa;Password=<tu-password>;TrustServerCertificate=True;"
  },
  "ApiXKey": "<tu-api-key>"
}
```

### 3. Aplicar migraciones

```bash
cd zombie-defense
dotnet ef database update --project ../zombie-defense.Infraestructure
```

### 4. Ejecutar la API

```bash
dotnet run --project zombie-defense
```

La API queda disponible en:

- HTTP: `http://localhost:5095`
- HTTPS: `https://localhost:7192`
- Documentación interactiva: `http://localhost:5095/scalar/v1`

---

## Endpoints

Todos los endpoints requieren el header `X-API-KEY`. Las rutas de documentación (`/scalar/*`, `/openapi/*`) están exentas.

### `GET /api/defense/optimal-strategy`

Calcula la estrategia óptima de eliminación dado un presupuesto de balas y tiempo.

**Query parameters:**

| Parámetro | Tipo | Requerido | Descripción |
|---|---|---|---|
| `bullets` | int | Sí | Número de balas disponibles |
| `secondsAvailable` | int | Sí | Segundos disponibles |

**Ejemplo de solicitud:**

```http
GET http://localhost:5095/api/defense/optimal-strategy?bullets=100&secondsAvailable=60
X-API-KEY: X-API-KEY-ZombieDefense206!
```

**Ejemplo de respuesta:**

```json
{
  "totalScore": 350,
  "bulletsUsed": 98,
  "secondsUsed": 58,
  "eliminatedZombies": [
    {
      "zombieTypeId": 1,
      "type": "Runner",
      "bulletsNeeded": 10,
      "shootingTime": 5,
      "score": 50,
      "threatLevel": "HIGH",
      "killCount": 3
    }
  ]
}
```

Los zombis en `eliminatedZombies` se ordenan por nivel de amenaza: `HIGH` → `MEDIUM` → `LOW`.

---

### `GET /api/simulations`

Retorna el historial completo de simulaciones ejecutadas, ordenado por `TotalScore` descendente.

**Ejemplo de solicitud:**

```http
GET http://localhost:5095/api/simulations
X-API-KEY: X-API-KEY-ZombieDefense206!
```

**Ejemplo de respuesta:**

```json
[
  {
    "id": 1,
    "date": "2024-01-15T10:30:00",
    "timeAvailable": 60,
    "bulletsAvailable": 100,
    "totalScore": 350,
    "eliminatedZombies": [...]
  }
]
```

---

### Documentación interactiva

Disponible solo en entorno `Development`:

| Ruta | Descripción |
|---|---|
| `GET /scalar/v1` | Scalar UI — interfaz interactiva |
| `GET /openapi/v1.json` | Esquema OpenAPI (JSON) |

---

## Autenticación

El API utiliza un middleware custom (`ApiKeyMiddleware`) que valida el header `X-API-KEY` en todas las solicitudes.

| Situación | Código de respuesta |
|---|---|
| Header ausente | `401 Unauthorized` |
| Clave inválida | `403 Forbidden` |
| Clave válida | Pasa al siguiente middleware |

La clave se configura en `appsettings.Development.json` bajo la propiedad `ApiXKey`.

> **Nota:** Para producción se recomienda externalizar esta configuración (variables de entorno, Azure Key Vault, etc.) y restringir los orígenes CORS.

---

## Esquema de base de datos

```
ZombieType
├── Id (PK)
├── Type (unique, max 100)
├── ShootingTime (int > 0)
├── BulletsNeeded (int > 0)
├── Score (int > 0)
├── ThreatLevel (HIGH | MEDIUM | LOW)
└── CreatedAt

Simulation
├── Id (PK)
├── Date
├── TimeAvailable (int > 0)
├── BulletsAvailable (int > 0)
├── TotalScore
└── CreatedAt

EliminatedZombie
├── Id (PK)
├── ZombieTypeId (FK → ZombieType, RESTRICT)
├── SimulationId (FK → Simulation, CASCADE)
├── PointsEarned
└── Timestamp

AuditLog
├── Id (PK)
├── TableName
├── Operation (INSERT | UPDATE | DELETE)
├── OldData (nullable)
├── NewData (nullable)
├── DbUser
├── AppUser (nullable)
└── Timestamp
```

---

## Estructura del proyecto

```
zombie-defense.slnx
├── zombie-defense/                         # Proyecto API (entry point)
│   ├── Controllers/
│   │   ├── DefenseController.cs
│   │   └── SimulationsController.cs
│   ├── Middleware/
│   │   └── ApiKeyMiddleware.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
│
├── zombie-defense.Application/             # Casos de uso y DTOs
│   ├── useCases/
│   │   ├── OptimalStrategy.cs
│   │   ├── CalculateOptimalStrategyUseCase.cs
│   │   ├── SaveSimulationUseCase.cs
│   │   └── HistoryUseCase.cs
│   └── DTOs/
│       ├── StrategyResult.cs
│       └── ZombieKillSummary.cs
│
├── zombie-defense.Domain/                  # Entidades y contratos
│   ├── Entities/
│   │   ├── ZombieType.cs
│   │   ├── Simulation.cs
│   │   ├── EliminatedZombie.cs
│   │   └── AuditLog.cs
│   └── Ports/
│       ├── IZombieTypeRepository.cs
│       └── ISimulationRepository.cs
│
└── zombie-defense.Infraestructure/         # Implementaciones y persistencia
    ├── Adapters/
    │   ├── SqlZombieTypeRepository.cs
    │   └── SqlSimulationRepository.cs
    └── Persistence/
        └── AppDbContext.cs
```
