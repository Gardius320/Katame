# Katame

App full-stack simple pero estable para gestionar finanzas, entrenamiento, tareas, metas,
proyectos y suscripciones. Ver [SPEC.md](SPEC.md) para la especificación técnica completa.

**Estado actual:** base de datos, autenticación (login + refresh token) y navegación por
módulos están funcionando de punta a punta. El único módulo con CRUD completo es
**Tareas**; el resto de los módulos muestran una pantalla "Próximamente" mientras se
implementan en las siguientes iteraciones.

## Stack

- **Backend:** ASP.NET Core Web API (.NET 8) + Entity Framework Core + MySQL (Pomelo)
- **Frontend:** React + Vite + TypeScript + Tailwind CSS + shadcn/ui
- **Base de datos:** MySQL 8 en Docker

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- La herramienta `dotnet-ef` instalada globalmente: `dotnet tool install --global dotnet-ef`

## 1. Variables de entorno

Copia el archivo de ejemplo en la raíz del proyecto y completa las contraseñas:

```bash
cp .env.example .env
```

```env
# .env (raíz del proyecto, usado por docker-compose)
MYSQL_ROOT_PASSWORD=elige-una-contraseña
MYSQL_USER=katame_app
MYSQL_PASSWORD=elige-otra-contraseña
```

El frontend usa su propio `.env` (en `frontend/`), basado en `frontend/.env.example`:

```env
# frontend/.env
VITE_API_BASE_URL=http://localhost:5057/api
```

## 2. Levantar MySQL con Docker

Desde la raíz del proyecto:

```bash
docker compose up -d
```

Esto crea el contenedor `katame-mysql-1` con la base de datos `katame`, expuesta en
`localhost:3306`, con los datos persistidos en un volumen nombrado.

## 3. Configurar los secretos del backend

El backend **nunca** lee la cadena de conexión ni la clave JWT desde `appsettings.json`.
Se configuran vía User Secrets (usa las mismas credenciales que pusiste en `.env`):

```bash
cd backend/KatameApi
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=katame;user=katame_app;password=TU_MYSQL_PASSWORD;"
dotnet user-secrets set "Jwt:Key" "una-clave-larga-y-aleatoria-de-al-menos-32-caracteres"
```

## 4. Aplicar las migraciones de Entity Framework

Con MySQL arriba y los secretos configurados:

```bash
cd backend/KatameApi
dotnet ef database update
```

Esto crea el esquema y siembra un usuario inicial:

- **Usuario:** `admin`
- **Contraseña:** `Admin123!`

Cámbiala después de tu primer login.

## 5. Certificado HTTPS de desarrollo

Para que `dotnet run` sirva HTTPS localmente sin advertencias del navegador:

```bash
dotnet dev-certs https --trust
```

## 6. Correr el backend

```bash
cd backend/KatameApi
dotnet run
```

La API queda disponible en `http://localhost:5057` (o `https://localhost:7100`).
Swagger UI: `http://localhost:5057/swagger`. Health check: `GET /health`.

## 7. Correr el frontend

```bash
cd frontend
npm install
npm run dev
```

La app queda disponible en `http://localhost:5173`.

## Pruebas

```bash
# Backend (xUnit)
cd backend
dotnet test

# Frontend (lint + build)
cd frontend
npm run lint
npm run build
```

## Estructura del proyecto

```
Katame/
├── docker-compose.yml       # MySQL 8
├── backend/
│   ├── KatameApi/           # API (Controllers, Services, Repositories, Models, DTOs, Data)
│   └── KatameApi.Tests/     # xUnit
└── frontend/
    └── src/
        ├── features/        # today, finance, training, tasks, goals, projects, subscriptions
        └── shared/           # componentes UI, api client, i18n (es.ts), stores de Zustand
```

## Convenciones

- Todo el código (clases, endpoints, carpetas, columnas) está en **inglés**.
- Todo el texto visible para el usuario está en **español**, centralizado en
  [`frontend/src/shared/i18n/es.ts`](frontend/src/shared/i18n/es.ts).
- Los mensajes de error del backend (FluentValidation, middleware de excepciones) también
  se devuelven en español, listos para mostrarse en un toast.
