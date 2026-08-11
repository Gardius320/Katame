# Katame — Prompt de continuación

Continuemos el desarrollo de Katame (spec completo en `SPEC.md`, en la raíz del repo). **El orden de implementación completo de `SPEC.md` (sección 7, los 10 puntos) ya está terminado.** Esto es lo que ya está hecho y lo que podría seguir (fase 2, opcional).

## Estado del entorno

- MySQL 8 corriendo en Docker (`docker-compose up -d`), contenedor `katame-mysql-1`.
- Backend: `backend/KatameApi`, corre en `http://localhost:5057` (`ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://localhost:5057`). User Secrets ya configurados (connection string + JWT key).
- Frontend: `frontend/`, Vite dev server en `http://localhost:5173` (`npm run dev`).
- Usuario semilla: `admin` / `Admin123!`.
- **Importante**: si el dev server de Vite lleva muchas horas corriendo, puede quedar sirviendo módulos obsoletos vía HMR. Si algo no refleja un cambio reciente, reiniciar el proceso de `npm run dev` lo resuelve.
- **Para probar desde el celular**: `Cors:AllowedOrigin` acepta una lista separada por comas (ej. `Cors__AllowedOrigin="http://localhost:5173,http://192.168.1.7:5173"`), y hay que levantar el backend con `--urls http://0.0.0.0:5057` y el frontend con `npm run dev -- --host 0.0.0.0` más `VITE_API_BASE_URL=http://<ip-lan>:5057/api`. El celular debe estar en la misma red (Wi-Fi/router) que la PC.
- **Para probar la instalación real de la PWA**: el manifest y el service worker solo se generan en la build de producción (`devOptions.enabled: false` en `vite.config.ts`). Hay que correr `VITE_API_BASE_URL=http://<ip-lan>:5057/api npm run build` y luego `npm run preview -- --host 0.0.0.0 --port 5173` (no `npm run dev`) para que el celular pueda instalar la app con ícono, manifest y offline caching activos.
- **Cuidado al reiniciar el backend durante una sesión de navegador activa**: invalida el refresh token en memoria/DB de esa sesión y la próxima petición autenticada redirige a `/login` silenciosamente (parece "se cerró sesión sola"). Solo hay que volver a loguearse; no es un bug.

## Ya completado y commiteado (17 commits, Conventional Commits)

1. `chore`: scaffold del proyecto (docker-compose, CI, README, SPEC.md, .gitignore)
2. `feat(backend)`: EF Core + auth JWT con refresh tokens, Swagger, Serilog, health check, rate limiting, FluentValidation en español
3. `feat(backend)`: módulo Tasks (TaskItem, soft delete, CRUD)
4. `feat(frontend)`: scaffold (Tailwind v4, shadcn/ui, sistema de diseño exacto del spec, dark mode, Zustand, React Query, Axios con refresh automático, React Router, PWA, ErrorBoundary, MSW/Vitest)
5. `feat(frontend)`: feature Tasks completo (lista, crear/editar, sello de finalización, toasts, skeleton)
6. `fix(frontend)`: pantalla de login con wordmark "KATAME" + tagline según spec
7. `feat(backend)`: módulo Training (TrainingDay + Exercise, seed de 3 días, endpoints anidados)
8. `feat(frontend)`: feature Training completo (días editables + ejercicios)
9. `feat(backend)`: módulo Finance (transactions, savings, obligations, credit cards) — 32 tests xUnit pasando
10. `feat(frontend)`: feature Finance completo (transactions/savings/obligations/credit-cards) — verificado end-to-end en navegador, incluyendo crear/editar/eliminar tarjetas de crédito
11. `feat(backend)`: módulo Subscriptions (Subscription: BaseEntity, soft delete) — 36 tests xUnit pasando, CORS ahora acepta múltiples orígenes separados por coma
12. `feat(frontend)`: feature Subscriptions completo — lista, crear/editar/eliminar, toggle de recordatorio, badge "Renueva pronto" (dentro de 7 días) — verificado end-to-end en navegador
13. `feat(backend)`: módulos Goals (sin soft delete, ProgressPercentage) y Projects (BaseEntity, soft delete, Status: active/on_hold/completed) — 45 tests xUnit pasando
14. `feat(frontend)`: features Goals (barra de progreso + sello al 100%, badge de categoría) y Projects (Select de estado, descripción) — verificado end-to-end en navegador
15. `feat(backend)`: endpoint agregador `GET /api/today` (saldo + tendencia 7 días, próximos vencimientos, entrenamiento del día, tareas urgentes) — 49 tests xUnit pasando
16. `feat(frontend)`: pantalla "Hoy" completa con 4 tarjetas (saldo con mini-gráfica Recharts, próximos vencimientos, entrenamiento de hoy, tareas urgentes) — verificado end-to-end en navegador con datos sembrados
17. `feat(frontend)`: ícono definitivo de PWA (marca "K" vectorial sin dependencia de fuentes, versión `any` y `maskable` con zona segura respetada, PNGs a 192/512px), apple-touch-icon y meta tags de iOS para instalación en pantalla de inicio, `lang: es` en el manifest

Todo verificado end-to-end en navegador contra el backend real, y con tests xUnit + Vitest en verde en cada paso (backend 49/49, frontend 5/5, lint solo warnings preexistentes de shadcn/ui, build OK). El manifest y el service worker se verificaron sirviendo la build de producción real (`vite preview`), con el service worker registrándose y activándose correctamente.

## Notas de aprendizaje (para no repetir investigación)

- **Los clicks del tool `computer` a veces no registran correctamente** en componentes Radix portados (Select, Tabs, Dialog, botones icon-only con `aria-label`, checkboxes) en este entorno de browser de pruebas — el patrón confiable que uso es disparar eventos con JavaScript directamente vía `javascript_tool`: `pointerdown` + `mousedown` + `pointerup` + `mouseup` + `click` con las coordenadas reales del `getBoundingClientRect()` del elemento, buscando el botón por `getAttribute('aria-label')` cuando el texto visible está vacío (icon-only), o por `textContent.trim()` cuando tiene texto. Un `.click()` simple a veces funciona para botones normales con texto, pero no es confiable — mejor usar siempre el patrón de eventos sintéticos para botones de submit/acción dentro de diálogos.
- **`Select` de shadcn/ui (Radix)**: el trigger es un `<button role="combobox">`. Ni el click normal ni el sintético garantizan abrirlo de forma consistente — a veces requiere disparar el click sintético y luego verificar `aria-expanded`/`data-state` por separado (puede abrirse "tarde" respecto al mismo tick de JS). Una vez abierto, las opciones son `[role="option"]` y sí responden bien al click sintético estándar.
- Los **refs de `read_page` pueden quedar obsoletos** después de escribir en un input si el componente re-renderiza (aunque el layout se vea igual) — si el texto tipado aparece concatenado en el campo equivocado (ej. "TítuloCategoría30" todo en el primer input), es señal de que el foco no se movió realmente. La solución más confiable es usar JS directo con el native setter (`Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value').set.call(el, valor)` + `dispatchEvent(new Event('input', {bubbles:true}))`) sobre `document.querySelectorAll('input')[i]`, en vez de encadenar `computer.click` + `computer.type` en varios campos seguidos.
- Los inputs numéricos de los formularios (`z.number()` + `field.onChange(e.target.valueAsNumber)`) suelen traer un valor por defecto (ej. "1") — usar `triple_click` para seleccionar todo antes de `type`, si no el texto se concatena al valor existente (ej. "1" + "15" = "115").
- Los inputs `type="date"` a veces no reciben bien el `type` simulado del tool `computer` — si el valor queda vacío, usar el native setter + evento `input` vía JS (mismo patrón de arriba).
- Cuando hay más de un `Dialog`/`AlertDialog` montados en el DOM simultáneamente (aunque uno esté cerrado), hay que filtrar por `[data-state="open"]` en el selector, porque Radix no desmonta el contenido cerrado inmediatamente.
- `javascript_tool` ejecuta cada llamada en el mismo scope global de la página — declarar `const`/`let` a nivel superior en más de una llamada da `SyntaxError: Identifier already declared`. Envolver el código en un IIFE `(function(){ ... })()` evita el problema.
- El mensaje de consola "ReferenceError: CardDescription is not defined" que a veces aparece en `read_console_messages` es un residuo obsoleto del buffer de la herramienta de un bug ya arreglado hace rato (confirmado con `grep` que no existe en el código) — no es un error real, ignorarlo.
- **Fix importante de EF Core/MySQL**: `ConfigureConventions` en `KatameDbContext.cs` con `configurationBuilder.Properties<decimal>().HavePrecision(18, 2)` — sin esto, MySQL/Pomelo genera `decimal(65,30)` por defecto y los montos salen con 25+ ceros decimales.
- **Fix de tipos en formularios**: con `react-hook-form` + `zodResolver` + campos numéricos, usar `z.number()` normal (sin `z.coerce.number()`) y en cada campo numérico reemplazar `{...field}` por props explícitas con `onChange={(e) => field.onChange(e.target.valueAsNumber)}`. `z.coerce.number()` rompe la inferencia de tipos (TS2322 en cascada), y usar `z.input`/`z.output` con 3 genéricos en `useForm` rompe el tipo de `value` en los `<Input>` a `unknown`.
- **CORS y refresh token al probar en LAN**: si el backend solo permite un origen (`Cors:AllowedOrigin`) y el navegador de pruebas está en un origen distinto al configurado, las llamadas a la API fallan silenciosamente y la app redirige a `/login` (se ve como "sesión expirada" pero es un bloqueo de CORS). `Cors:AllowedOrigin` ahora acepta una lista separada por comas para cubrir `localhost` + la IP LAN al mismo tiempo.
- **`curl` en Git Bash de Windows y acentos**: pasar `ñ`/tildes en el body JSON de `curl -d '...'` a veces llega mal codificado al backend y produce un error de deserialización JSON que parece un bug del API pero es un problema de encoding de la terminal — si un POST/PUT falla raro con texto en español, probar el mismo payload sin caracteres especiales para descartarlo antes de investigar el backend.
- **`/api/today` reutiliza repositorios existentes** (`ITransactionRepository`, `IObligationRepository`, `ICreditCardRepository`, `ISubscriptionRepository`, `ITrainingDayRepository`, `ITaskRepository`) en vez de crear una entidad nueva — es un servicio puramente de agregación (`TodayService`). El "próximo pago de tarjeta" se calcula a partir de `CreditCard.PaymentDay` (día del mes) con `GetNextOccurrence`, ya que el modelo no guarda un saldo pendiente de tarjeta. Ventanas usadas: 14 días para vencimientos próximos, 1 día (hoy/mañana) para tareas urgentes, 7 días para la tendencia de saldo.
- **Rasterizar SVG a PNG sin depender de fuentes del sistema**: el ícono original usaba `<text>` con "Space Grotesk", lo cual depende de que esa fuente esté instalada donde se rasterice (nunca garantizado en un entorno de build/CI). La solución fue rediseñar el ícono como trazos vectoriales puros (`<path>` con `stroke`, sin texto), eliminando la dependencia de fuentes por completo. Para rasterizar, `npx --yes resvg-cli` funciona bien en Windows/Git Bash, pero su sintaxis real es `--fit-width <px> input.svg output.png` (no `--width`/`--height`, a pesar de lo que sugeriría el nombre del paquete).
- **Ícono maskable**: la "zona segura" es un círculo de diámetro ~80% del tamaño del ícono centrado — el contenido importante (no el fondo) debe cuidarse dentro de ese círculo, o algunos launchers Android lo recortan. El fondo de la versión maskable debe ser un cuadrado a sangre completa sin esquinas redondeadas (el sistema operativo aplica su propia máscara).
- **El manifest y el service worker de `vite-plugin-pwa` no se generan en `npm run dev`** (con `devOptions.enabled: false`, que es la config actual) — para probar instalación real hay que compilar y servir con `vite preview`, no con el dev server.

No implementar todavía (según spec, fase 2 opcional, no forma parte del orden de implementación): Hangfire, Dockerfile del backend, Storybook, Husky, Playwright.

## Siguiente paso sugerido

El proyecto cumple el alcance completo definido en `SPEC.md`. Los siguientes pasos son a discreción del usuario:
- Confirmar que la instalación de la PWA en el celular quedó bien (ícono, nombre, modo standalone sin barra del navegador).
- Si se quiere seguir iterando, la fase 2 de la spec (Hangfire para recordatorios precalculados, Dockerfile del backend, Storybook, Husky, Playwright) es la lista natural de "qué sigue", pero ninguno de esos ítems es obligatorio — el spec explícitamente los dejó fuera del alcance inicial.
