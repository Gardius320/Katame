# Katame — Prompt de continuación

Katame es una app personal (tareas, entrenamiento, finanzas, suscripciones, metas, proyectos, pantalla "Hoy") con backend .NET + frontend React/Vite, PWA instalable. **El alcance completo de `SPEC.md` ya está implementado**, y desde esta sesión **el proyecto también está desplegado en producción** (no depende de que la PC del usuario esté prendida).

## Estado de producción (desplegado y verificado end-to-end)

- **Frontend:** `https://katame-plum.vercel.app` (Vercel, deploy automático desde GitHub, root directory `frontend`).
- **Backend:** `https://katame-production.up.railway.app` (Railway, Dockerfile en `backend/KatameApi/Dockerfile`, root directory `backend/KatameApi`).
- **Base de datos:** MySQL en Railway (plugin del mismo proyecto), **separada y distinta** de la base local en Docker del usuario — tiene solo la semilla original (`admin` / `Admin123!`), no los datos de prueba que se acumularon en local.
- **Repo:** `https://github.com/Gardius320/Katame` (rama `master`). Railway y Vercel están conectados a esa rama — **cualquier `git push` a `master` dispara redeploy automático en ambos**, sin pasos manuales.
- Confirmado con browser real: login, registro, PWA (manifest + service worker) todo funcionando en la URL pública.

### Variables de entorno configuradas en Railway (servicio "Katame")
- `ConnectionStrings__DefaultConnection` = `server=${{MySQL.MYSQLHOST}};port=${{MySQL.MYSQLPORT}};database=${{MySQL.MYSQLDATABASE}};user=${{MySQL.MYSQLUSER}};password=${{MySQL.MYSQLPASSWORD}};` (referencia viva al servicio MySQL del mismo proyecto)
- `Jwt__Key` = clave de 48 bytes generada para producción (distinta a la de user-secrets local)
- `Cors__AllowedOrigin` = `http://localhost:5173,https://katame-plum.vercel.app`
- `Frontend__BaseUrl` = `https://katame-plum.vercel.app` (arma el link del correo de recuperación de contraseña)

### Variables de entorno configuradas en Vercel
- `VITE_API_BASE_URL` = `https://katame-production.up.railway.app/api`

### Pendiente para que el correo de recuperación de contraseña funcione de verdad
Faltan `Email__SenderEmail` y `Email__SenderPassword` en Railway (contraseña de aplicación de Gmail, no la contraseña real de la cuenta — se genera en `myaccount.google.com/apppasswords`). Sin esto, `EmailService` falla en silencio (logea el error pero no rompe el endpoint) — el token de reseteo se genera igual, solo no se envía el correo real.

## Cómo actualizar el proyecto de acá en adelante

1. Editar código localmente.
2. Probar local (`dotnet test` en `backend/`, `npx vitest run` + `npx tsc --noEmit` en `frontend/`).
3. `git commit` + `git push` a `master`.
4. Railway y Vercel redeployan solos. Si el cambio incluye una migración EF Core nueva, se aplica sola al arrancar el backend (ver más abajo).

No hace falta tocar los dashboards de Railway/Vercel salvo para variables de entorno nuevas, ver logs, o diagnosticar un fallo de deploy.

## Completado en esta sesión (además del alcance original de `SPEC.md`)

Historial completo en `git log` (rama `master`, 20 commits). Los dos más recientes son de esta sesión:

1. **`9ccb2ce` feat(auth): self-registration, password recovery, and admin user management**
   - Login personalizado: toast "Bienvenid@ de nuevo, {Nombre}" usando `firstName` (antes decía solo "Bienvenido de nuevo" genérico).
   - Registro rediseñado: pide Nombre, Apellido, Cédula, Correo, Teléfono, Contraseña, Confirmar contraseña — **sin campo de usuario**. El username se autogenera del correo (parte antes de `@`, con sufijo numérico si ya existe) y se le muestra al usuario tras registrarse. El login sigue siendo Usuario + Contraseña, sin cambios — no rompe las credenciales existentes.
   - Validación real de Cédula ecuatoriana (dígito verificador, algoritmo módulo 10) y de Teléfono (formato celular `09XXXXXXXX` / fijo `0X XXXXXXX`), tanto en backend (FluentValidation + `EcuadorianDocumentId.cs`) como en frontend (mismo algoritmo replicado en `shared/lib/ecuadorian-document-id.ts`, con Zod).
   - Rate limiting en `/api/auth/register` (política separada `"register"`, 3/min).
   - Flujo de recuperación de contraseña completo: `/api/auth/forgot-password` + `/api/auth/reset-password`, token de un solo uso con expiración de 30 min, respuesta genérica (no filtra si el correo existe), rate limit propio `"password-reset"` (5/min, separado de `"register"` — importante, si comparten política un usuario legítimo que pide el link y resetea en el mismo minuto se bloquea a sí mismo). Páginas `/forgot-password` y `/reset-password` en el frontend.
   - Se integró y terminó el **panel de administración de Usuarios** (`UsersController`, `UserService`, DTOs, validators, feature `frontend/src/features/users`) que ya existía en el working tree pero **nunca se había commiteado** — se descubrió al revisar `git status` al pedir el commit de esta sesión. Se le agregaron los mismos campos personales (Nombre, Apellido, Cédula, Teléfono) para mantener consistencia con el registro.
   - Cobertura de tests que antes no existía: `AuthServiceTests` (0 → 14 tests: registro, login, refresh, forgot/reset password, generación de username único), validators de Register/ForgotPassword/ResetPassword/CreateUser/UpdateUser, y `EcuadorianDocumentIdTests`. Backend pasó de 74 a **136 tests**, todos en verde.

2. **`36a7fc3` feat(backend): add production Dockerfile and auto-apply migrations on startup**
   - `Dockerfile` de producción (multi-stage, SDK 8.0 → aspnet runtime 8.0) + `.dockerignore`. El puerto se resuelve en el entrypoint vía `${PORT:-8080}` (Railway inyecta `PORT` en runtime, no en build — no se puede fijar con un `ENV` normal).
   - `Database.Migrate()` al arrancar `Program.cs`, gateado implícitamente por no haber otro paso de deploy que corra `dotnet ef database update` contra Railway.

## Notas de aprendizaje (para no repetir investigación)

### Deploy en Railway (monorepo)
- Railway necesita que **"Root Directory"** en Settings del servicio apunte a `backend/KatameApi` explícitamente — si no, intenta buildear desde la raíz del repo (que tiene backend Y frontend juntos) y el build falla en segundos sin encontrar el Dockerfile.
- Las variables del plugin de MySQL de Railway usan el formato **sin guion bajo**: `MYSQLHOST`, `MYSQLPORT`, `MYSQLUSER`, `MYSQLPASSWORD`, `MYSQLDATABASE` (hay también variantes con guion bajo como `MYSQL_DATABASE`/`MYSQL_ROOT_PASSWORD`, pero para armar la connection string ADO.NET son las sin guion bajo). Se referencian desde otro servicio con `${{NombreDelServicio.VARIABLE}}` (ej. `${{MySQL.MYSQLHOST}}`), sintaxis que Railway resuelve en runtime — hay que escribir el nombre exacto del servicio como aparece en el panel (en este proyecto es literalmente `MySQL`).
- El panel de Railway distingue **"Build Logs"** (fase de armado de la imagen) de **"Deploy Logs"** (salida del proceso ya corriendo) — el error real de `InvalidOperationException` por falta de config solo aparece en Deploy Logs, no en Build Logs.
- Un deployment puede figurar como "successful"/"ACTIVE" en la fase de build y aun así estar crasheando en runtime — siempre verificar el estado real del servicio (punto verde "Online" vs rojo "Crashed" en el panel lateral), no solo el badge del deployment.
- Exponer el servicio públicamente requiere un paso aparte: Settings → Networking → "Generate Domain", eligiendo el puerto que expone el Dockerfile (8080 en este proyecto).

### Arquitectura del proyecto
- A diferencia de otros proyectos del usuario (ej. PetCare-Monorepo, que usa Clean/Onion Architecture con proyectos `.csproj` separados para Domain/Application/Infrastructure/API), Katame es intencionalmente **un solo proyecto** (`KatameApi.csproj`) con capas por carpeta (Controllers/Services/Repositories/Models/DTOs/Validators). Es una decisión de escala válida para una app personal de un solo desarrollador, no una carencia — si el proyecto creciera mucho o pasara a tener varios devs, migrar a Clean Architecture sería la siguiente movida natural, pero no antes.

### Notas heredadas de sesiones anteriores (siguen vigentes)
- **Clicks del tool `computer`** a veces no registran en componentes Radix (Select, Tabs, Dialog, botones icon-only) — usar `javascript_tool` con eventos sintéticos (`pointerdown`+`mousedown`+`pointerup`+`mouseup`+`click`) sobre las coordenadas reales de `getBoundingClientRect()`.
- **Inputs de formularios**: usar el native setter (`Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set`) + evento `input` en vez de encadenar `computer.click`+`computer.type`, para evitar que el texto se concatene en el campo equivocado tras un re-render.
- **Fix de EF Core/MySQL**: `ConfigureConventions` con `HavePrecision(18, 2)` para `decimal` — sin esto MySQL/Pomelo usa `decimal(65,30)` por defecto.
- **CORS + refresh token**: si el origen del frontend no está en `Cors:AllowedOrigin`, las llamadas fallan silenciosamente y la app redirige a `/login` pareciendo "sesión expirada" pero es un bloqueo de CORS — confirmar la consola del navegador antes de asumir que es un bug de auth.
- El manifest y el service worker de `vite-plugin-pwa` **no se generan en `npm run dev`** (`devOptions.enabled: false`) — solo en build de producción. Esto ya no es un problema en producción real (Vercel), pero sigue aplicando si se quiere probar la instalación de la PWA sirviendo local con `vite preview`.

## Siguiente paso sugerido

- **Conseguir la contraseña de aplicación de Gmail** y cargarla en Railway (`Email__SenderEmail`, `Email__SenderPassword`) para que el correo de recuperación de contraseña se envíe de verdad — es lo único que quedó a medio camino.
- Instalar la PWA en el celular desde `https://katame-plum.vercel.app` (confirmado que el manifest + ícono + service worker se sirven bien) y confirmar que se vea como un ícono nativo en el escritorio.
- Más allá de eso, el proyecto cumple el alcance completo de `SPEC.md` y ya está en producción. La fase 2 (opcional, nunca fue parte del alcance obligatorio) sigue siendo: Hangfire para recordatorios precalculados, Storybook, Husky, Playwright.
