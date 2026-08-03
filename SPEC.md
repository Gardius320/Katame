# Katame — Especificación técnica

App instalable (PWA) para gestionar finanzas, entrenamiento, tareas, metas, proyectos y suscripciones. Arquitectura cliente-servidor para correr en tu PC local.

**Stack decidido:** React (frontend) + C# / ASP.NET Core (backend) + MySQL (base de datos)

**Misión del proyecto:** simple pero estable. Ante cualquier duda de alcance durante el desarrollo, priorizar que lo que existe funcione bien y sin bugs sobre agregar más funcionalidad.

**Convención de nomenclatura:** todo el código (clases, propiedades, endpoints, rutas de carpetas, nombres de tablas/columnas, valores de enum) va en **inglés**, siguiendo la convención estándar de la industria. Todo lo que ve el usuario (labels, botones, toasts, mensajes de error, nombres de módulos en la UI) va en **español**. En el frontend, esos textos se centralizan en un archivo de strings (ej. `src/shared/i18n/es.ts`) en vez de quedar hardcodeados dentro de cada componente — así, si algún día quieres otro idioma, solo agregas otro archivo.

Ejemplo concreto: la entidad se llama `Task` con `Status` en `{pending, in_progress, done}`, pero en la pantalla el usuario ve "Tarea" con estado "Pendiente" — el mapeo inglés→español vive en el archivo de strings del frontend, no en el backend.

---

## 1. Stack tecnológico

### Frontend
| Capa | Elección | Por qué |
|---|---|---|
| Framework | **React + Vite + TypeScript** | Arranque rápido, tipado seguro |
| Estilos | **Tailwind CSS** | Consistente con el prototipo ya hecho |
| Componentes UI | **shadcn/ui** (sobre Radix UI) | Base de componentes accesibles y personalizables (modales, inputs, cards) para los formularios de los 7 módulos, sin armarlos desde cero |
| Toast | **Sonner** | Confirmaciones ("Guardado", "Eliminado") y errores de la API, en español |
| Skeleton | **Skeleton (shadcn/ui)** | Estados de carga mientras React Query trae datos del backend, en vez de spinners genéricos |
| Estado servidor | **TanStack React Query** | Los datos viven en el backend, no local — React Query maneja cache, refetch y estados de carga/error contra la API automáticamente |
| Estado UI | **Zustand** (con middleware `persist`) | Estado de interfaz (tab activo, modales, tema claro/oscuro) y el token JWT + datos del usuario logueado, persistidos en localStorage |
| Formularios | **React Hook Form + Zod** | Validación tipada de montos, fechas, campos requeridos; mensajes de error en español |
| Cliente HTTP | **Axios** | Interceptor que adjunta el header `Authorization: Bearer {token}` en cada request, renueva el token con el refresh token, y redirige a `/login` si la sesión expira |
| Gráficas | **Recharts** | Resumen de gastos/ahorro en Finanzas |
| Fechas | **date-fns** | Cálculo de recurrencias (cortes, renovaciones) |
| Iconos | **lucide-react** | Mismo set del prototipo |
| PWA | **vite-plugin-pwa** | Instalable en el celular/escritorio |
| Manejo de errores | **Error Boundary** (React) | Captura errores de renderizado dentro de un módulo sin tumbar toda la app |
| Config de entorno | **Variables de entorno** (`.env` + `import.meta.env` de Vite) | URL base de la API distinta en dev/producción, sin hardcodear `localhost:5000` |
| Debug | **React Query Devtools** | Panel para inspeccionar cache, estados de carga y refetch mientras desarrollas |
| Calidad de código | **ESLint + Prettier** | Consistencia de estilo y detección de errores comunes |
| Rendimiento | **Code splitting por ruta** (`React.lazy` + React Router) | Cada módulo se carga solo cuando lo visitas |
| Tema | **Dark mode** | shadcn/ui lo soporta nativo con la clase `dark:` de Tailwind; toggle guardado en el store de Zustand |
| Listas grandes | **TanStack Virtual** | Para la lista de transacciones si crece mucho |
| Testing frontend | **MSW (Mock Service Worker)** | Mockea la API en pruebas de frontend sin depender de que el backend esté corriendo |
| Documentación de componentes *(fase 2)* | **Storybook** | Documentar y probar visualmente los componentes de shadcn/ui de forma aislada |
| Pre-commit *(fase 2)* | **Husky + lint-staged** | Corre ESLint automáticamente antes de cada commit |
| Tests E2E *(fase 2)* | **Playwright** | Prueba el flujo completo: login → crear tarea → verla en "Hoy" |

### Backend
| Capa | Elección | Por qué |
|---|---|---|
| Framework | **ASP.NET Core Web API (.NET 8)** | Tipado fuerte, buen soporte de EF Core |
| ORM | **Entity Framework Core + Pomelo.EntityFrameworkCore.MySql** | Provider estable de EF Core para MySQL, migraciones Code-First |
| Arquitectura | **Controllers → Services → Repositories** | Separa reglas de negocio (Services) del acceso a datos (Repositories) |
| Documentación API | **Swagger / OpenAPI** (Swashbuckle) | Autogenera docs interactivas de cada endpoint |
| Autenticación | **JWT** (access + refresh token) | Login real con usuario/contraseña |
| Hash de contraseñas | **BCrypt.Net-Next** | Entidad `User` propia, sin todo ASP.NET Core Identity |
| Validación | **FluentValidation** | Reglas de validación de cada DTO separadas del modelo. Mensajes de error se traducen a español antes de llegar al frontend |
| Middleware | **Exception Handling Middleware** (custom) | Captura errores de validación y excepciones no controladas, devuelve `{ status, message, errors }` con mensajes en español |
| Logging | **Serilog** | Consola + archivo con rotación diaria, retención de 30 días |
| CORS | **Política CORS** en `Program.cs` | El frontend (`localhost:5173`) y el backend corren en puertos distintos |
| Mapeo | **AutoMapper** | Mapeo automático entre Entidades y DTOs |
| Refresh tokens | **Refresh Token** (tabla propia, ligado a `User`) | Renueva la sesión sin desloguear al usuario cada 15-30 min |
| Seeding | **EF Core Seed Data** | Usuario inicial + días de entrenamiento base al migrar |
| Paginación y filtros | **Query params + `Skip/Take`** | Sobre todo en `transactions` |
| Soft delete | **Campo `IsDeleted`** + query filter global | En vez de borrar registros de verdad |
| Secretos | **User Secrets** (`dotnet user-secrets`) | Cadena de conexión MySQL y clave JWT fuera del repo |
| Health check | **`/health`** | Confirma que la API y MySQL están vivas |
| Rate limiting | **`Microsoft.AspNetCore.RateLimiting`** en `/api/auth/login` | Sin límite, fuerza bruta sobre el login es trivial |
| HTTPS local | **`dotnet dev-certs https`** | Certificado de desarrollo, mismo comportamiento que producción |
| Zona horaria | **UTC en backend**, conversión a local solo en frontend | Evita que fechas de corte/vencimiento se corran un día |
| Exportar datos | **CSV export** en `transactions` | Sacar tus datos financieros a Excel |
| Backup | **`mysqldump` programado** (tarea de Windows/cron) | Tu info financiera no depende de un único punto de falla |
| Jobs en segundo plano *(fase 2)* | **Hangfire** | Precalcular recordatorios en vez de calcularlos en cada request |
| Contenerización *(fase 2)* | **Dockerfile del backend** | Complementa el `docker-compose` de MySQL |

### Base de datos
**MySQL 8 corriendo en Docker** (vía `docker-compose`, no instalación directa).

```yaml
# docker-compose.yml (en la raíz del proyecto)
services:
  mysql:
    image: mysql:8
    restart: unless-stopped
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
      MYSQL_DATABASE: katame
      MYSQL_USER: ${MYSQL_USER}
      MYSQL_PASSWORD: ${MYSQL_PASSWORD}
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql
volumes:
  mysql_data:
```

Las credenciales van en un `.env` (ignorado por Git) que alimenta tanto el `docker-compose.yml` como la cadena de conexión de EF Core vía User Secrets.

---

## 2. Arquitectura general

```
[React SPA] --HTTP/JSON--> [ASP.NET Core Web API] --EF Core--> [MySQL]
```

El frontend nunca toca la base de datos directamente — todo pasa por la API REST del backend. La PWA depende del backend para leer/escribir datos; no hay modo "offline total" a menos que se agregue cache local más adelante.

---

## 3. Estructura de carpetas

```
katame/
├── backend/
│   └── KatameApi/
│       ├── Controllers/        # TaskController, FinanceController, etc.
│       ├── Models/              # Entidades EF Core (en inglés)
│       ├── DTOs/
│       ├── Services/
│       ├── Repositories/
│       ├── Data/                # DbContext
│       ├── Migrations/
│       └── Program.cs
│   └── KatameApi.Tests/      # xUnit
├── frontend/
│   └── src/
│       ├── features/
│       │   ├── today/
│       │   ├── finance/
│       │   │   ├── transactions/
│       │   │   ├── savings/
│       │   │   ├── obligations/
│       │   │   └── credit-cards/
│       │   ├── training/
│       │   ├── tasks/
│       │   ├── goals/
│       │   ├── projects/
│       │   └── subscriptions/
│       ├── shared/
│       │   ├── components/
│       │   ├── api/             # cliente axios + hooks de React Query
│       │   ├── i18n/            # strings en español (es.ts)
│       │   └── utils/
│       ├── App.tsx
│       └── main.tsx
└── docker-compose.yml            # levanta MySQL con `docker-compose up -d`
```

---

## 4. Modelo de datos (entidades EF Core)

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }     // generado con BCrypt, nunca texto plano
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Patrón de soft delete: las entidades "eliminables" heredan de esta clase
// base, y un query filter global de EF Core las excluye de los SELECT.
public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Type { get; set; }        // "income" | "expense"
    public string Category { get; set; }
    public DateTime Date { get; set; }
}

public class SavingsGoal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? DueDate { get; set; }
}

public class Obligation : BaseEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsPaid { get; set; }
}

public class CreditCard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StatementDay { get; set; }    // día de corte del mes
    public int PaymentDay { get; set; }
    public decimal CreditLimit { get; set; }
}

public class TrainingDay
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string Title { get; set; }
    public List<Exercise> Exercises { get; set; } = new();
}

public class Exercise
{
    public int Id { get; set; }
    public int TrainingDayId { get; set; }
    public string Name { get; set; }
    public string SetsReps { get; set; }
}

// Nota: se nombra "TaskItem" y no "Task" para evitar el choque de nombres
// con System.Threading.Tasks.Task del propio .NET.
public class TaskItem : BaseEntity
{
    public string Title { get; set; }
    public string Status { get; set; }       // "pending" | "in_progress" | "done"
    public DateTime? Date { get; set; }
    public int? ProjectId { get; set; }
}

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime? DueDate { get; set; }
}

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
}

public class Subscription : BaseEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool ReminderEnabled { get; set; }
}
```

---

## 5. Convención de endpoints REST

| Módulo | Endpoints base |
|---|---|
| Auth | `POST /api/auth/login` (devuelve access + refresh token), `POST /api/auth/register`, `POST /api/auth/refresh` |
| Tasks | `GET/POST /api/tasks`, `PUT/DELETE /api/tasks/{id}` |
| Finance | `/api/finance/transactions` (soporta `?page&pageSize&startDate&endDate&category`), `/api/finance/transactions/export` (CSV), `/api/finance/savings`, `/api/finance/obligations`, `/api/finance/credit-cards` |
| Training | `/api/training/days`, `/api/training/days/{id}/exercises` |
| Goals | `/api/goals` |
| Projects | `/api/projects` |
| Subscriptions | `/api/subscriptions` |
| Today | `GET /api/today` — endpoint agregador que junta próximos vencimientos, entrenamiento del día y tareas urgentes en una sola llamada |
| Salud | `GET /health` — confirma que la API y la conexión a MySQL están arriba |

Todos los endpoints excepto `/api/auth/*` llevan el atributo `[Authorize]` en el backend. En el frontend, un componente `ProtectedRoute` de React Router revisa si hay token válido en el store de Zustand antes de mostrar cualquier módulo; si no, redirige a `/login`.

---

## 6. Recordatorios — límites reales

- **Dentro de la app:** el endpoint `/api/today` calcula qué vence pronto (tarjetas, obligaciones, suscripciones) y el frontend lo muestra como banner al abrir. Esto funciona siempre.
- **Notificación del sistema operativo aunque la app esté cerrada:** no es posible sin infraestructura de push (requiere un servicio externo tipo Firebase Cloud Messaging o Web Push con VAPID keys, expuesto públicamente — no aplica a un backend que solo corre en tu PC local).
- Alternativa simple: seguir usando **alarmas del calendario/teléfono** para fechas puntuales importantes, en paralelo al banner dentro de la app.

---

## 7. Orden de implementación sugerido

1. `docker-compose.yml` funcional con MySQL arriba
2. Backend: setup completo (EF Core + migración inicial + seeding), auth JWT con refresh tokens funcionando end-to-end, Swagger navegable
3. Frontend: setup completo (PWA instalable, sistema de diseño aplicado, dark mode, navegación por tabs, pantalla de login conectada al backend)
4. Módulo **Tasks** end-to-end (CRUD completo backend + frontend, con validación, toasts y skeleton) — valida toda la arquitectura antes de escalar
5. Módulo Training
6. Módulo Finance (transactions, savings, obligations, credit-cards)
7. Módulo Subscriptions
8. Módulos Goals y Projects
9. Endpoint + pantalla "Today"
10. Pulido PWA: ícono, splash screen, prueba de instalación en el celular

---

## 8. Prompt listo para Claude Cowork

```
Crea una solución full-stack llamada "katame". Misión del proyecto:
simple pero estable — ante cualquier duda de alcance, prioriza que lo implementado
funcione bien y sin bugs sobre agregar más funcionalidad.

CONVENCIÓN DE NOMBRES: todo el código (clases, propiedades, endpoints, rutas de
carpetas, columnas de base de datos, valores de enum) va en inglés. Todo el
texto visible para el usuario (labels, botones, toasts, mensajes de error) va
en español, centralizado en un archivo de strings en el frontend
(src/shared/i18n/es.ts) en vez de hardcodeado en cada componente. Los mensajes
de error que devuelve el backend (FluentValidation, middleware de excepciones)
también deben estar en español, ya que van directo al usuario vía toast.

═══════════════════════════════════════
BASE DE DATOS
═══════════════════════════════════════
MySQL 8 corriendo en Docker (no instalación directa). Crea un docker-compose.yml
en la raíz con un servicio "mysql" (imagen mysql:8), variables de entorno desde
.env (MYSQL_ROOT_PASSWORD, MYSQL_USER, MYSQL_PASSWORD), base de datos
"katame", puerto 3306 mapeado, y un volumen nombrado para persistir
los datos.

═══════════════════════════════════════
BACKEND — /backend/KatameApi (ASP.NET Core Web API, .NET 8)
═══════════════════════════════════════
Arquitectura en capas: Controllers, Services, Repositories, Models (entidades),
DTOs, Data (DbContext).

Librerías y piezas a integrar:
- Entity Framework Core + Pomelo.EntityFrameworkCore.MySql, con migraciones
  Code-First y datos semilla (usuario inicial + días de entrenamiento base)
- Swagger/OpenAPI (Swashbuckle) para documentación
- Autenticación JWT con access token + refresh token; contraseñas hasheadas
  con BCrypt.Net-Next; entidad User propia (sin ASP.NET Identity completo)
- FluentValidation para validar todos los DTOs de entrada, con mensajes de
  error en español
- Middleware global de manejo de excepciones que captura errores de
  FluentValidation y excepciones no controladas, devolviendo siempre
  { status, message, errors } con mensajes en español
- Serilog (consola + archivo con rotación diaria, retención de 30 días),
  integrado con el middleware de excepciones
- CORS configurado para el origen del frontend en desarrollo
- AutoMapper para mapear Entidades <-> DTOs
- Paginación y filtros (page, pageSize, startDate, endDate, category) en
  el endpoint de transactions, más un endpoint de exportación a CSV
- Soft delete: clase base BaseEntity con IsDeleted + query filter global de
  EF Core, aplicada a Transaction, TaskItem, Project, Obligation, Subscription
- Rate limiting (Microsoft.AspNetCore.RateLimiting) en POST /api/auth/login
- Manejo de fechas en UTC en toda la base de datos y la API
- Health check en GET /health
- Todos los endpoints excepto /api/auth/* protegidos con [Authorize]
- Configura User Secrets para la cadena de conexión de MySQL y la clave JWT
  (nunca en appsettings.json)

Entidades EF Core (con sus migraciones, nombradas en inglés):
- User (Id, Username, PasswordHash, RefreshToken?, RefreshTokenExpiry?, CreatedAt)
- BaseEntity (abstracta: Id, IsDeleted) — heredada por las entidades con soft delete
- Transaction : BaseEntity (Amount, Type, Category, Date)
- SavingsGoal (Name, TargetAmount, CurrentAmount, DueDate?)
- Obligation : BaseEntity (Name, Amount, DueDate, IsRecurring, IsPaid)
- CreditCard (Name, StatementDay, PaymentDay, CreditLimit)
- TrainingDay (DayOfWeek, Title, lista de Exercise)
- Exercise (TrainingDayId, Name, SetsReps)
- TaskItem : BaseEntity (Title, Status: pending/in_progress/done, Date?, ProjectId?)
  — se llama TaskItem y no Task para evitar choque con System.Threading.Tasks.Task
- Goal (Title, Category, ProgressPercentage, DueDate?)
- Project : BaseEntity (Name, Description, Status)
- Subscription : BaseEntity (Name, Amount, RenewalDate, ReminderEnabled)

Endpoints REST por módulo: /api/auth (login, register, refresh), /api/tasks,
/api/finance/transactions (+ /export), /api/finance/savings,
/api/finance/obligations, /api/finance/credit-cards, /api/training/days
(+ /{id}/exercises), /api/goals, /api/projects, /api/subscriptions,
/api/today (agregador: próximos vencimientos + entrenamiento del día + tareas
urgentes), /health.

Proyecto de tests: /backend/KatameApi.Tests con xUnit.

═══════════════════════════════════════
FRONTEND — /frontend (React + Vite + TypeScript)
═══════════════════════════════════════
Estructura por features (nombres de carpeta en inglés): today, finance
(transactions/savings/obligations/credit-cards), training (días editables
con ejercicios), tasks, goals, projects, subscriptions — cada una bajo
src/features/, más src/shared/ (components, api, i18n, utils).

Librerías a integrar:
- Tailwind CSS + shadcn/ui (sobre Radix UI) como base de componentes
- Sonner para toasts, Skeleton (shadcn/ui) para estados de carga
- TanStack React Query para consumir la API (con React Query Devtools en
  desarrollo), Zustand con middleware persist para estado de UI + sesión
  (token JWT y datos del usuario en localStorage)
- React Hook Form + Zod para formularios, con mensajes de validación en español
- Axios con interceptor que adjunta el header Authorization y redirige a
  /login en 401; usa el refresh token para renovar el access token
  automáticamente antes de que expire
- React Router con code splitting por r
```
