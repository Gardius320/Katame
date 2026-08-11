import { useState } from 'react'
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'
import {
  CalendarCheck,
  Dumbbell,
  FolderKanban,
  ListTodo,
  LogOut,
  Moon,
  MoreHorizontal,
  Repeat,
  Sun,
  Target,
  Users,
  Wallet,
  type LucideIcon,
} from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { useUiStore } from '@/shared/store/ui-store'
import { Button } from '@/shared/components/ui/button'
import { ThemeToggle } from '@/shared/components/theme-toggle'
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from '@/shared/components/ui/sheet'
import { cn } from '@/shared/lib/utils'

const ICON_STROKE_WIDTH = 1.75
const MOBILE_PRIMARY_COUNT = 4

const navItems: { to: string; label: string; icon: LucideIcon }[] = [
  { to: '/today', label: es.nav.today, icon: CalendarCheck },
  { to: '/finance', label: es.nav.finance, icon: Wallet },
  { to: '/training', label: es.nav.training, icon: Dumbbell },
  { to: '/tasks', label: es.nav.tasks, icon: ListTodo },
  { to: '/goals', label: es.nav.goals, icon: Target },
  { to: '/projects', label: es.nav.projects, icon: FolderKanban },
  { to: '/subscriptions', label: es.nav.subscriptions, icon: Repeat },
]

const adminNavItem: { to: string; label: string; icon: LucideIcon } = {
  to: '/users',
  label: es.nav.users,
  icon: Users,
}

function getInitials(username: string): string {
  return username.slice(0, 2).toUpperCase()
}

export function AppShell() {
  const navigate = useNavigate()
  const location = useLocation()
  const username = useSessionStore((state) => state.username)
  const isAdmin = useSessionStore((state) => state.isAdmin)
  const clearSession = useSessionStore((state) => state.clearSession)
  const theme = useUiStore((state) => state.theme)
  const toggleTheme = useUiStore((state) => state.toggleTheme)
  const [moreOpen, setMoreOpen] = useState(false)

  const handleLogout = () => {
    clearSession()
    navigate('/login', { replace: true })
  }

  const visibleNavItems = isAdmin ? [...navItems, adminNavItem] : navItems
  const mobilePrimaryItems = visibleNavItems.slice(0, MOBILE_PRIMARY_COUNT)
  const mobileMoreItems = visibleNavItems.slice(MOBILE_PRIMARY_COUNT)
  const moreActive = mobileMoreItems.some((item) => location.pathname.startsWith(item.to))

  return (
    <div className="flex min-h-screen bg-background text-foreground">
      {/* Sidebar: solo en pantallas medianas y grandes (escritorio) */}
      <aside className="hidden w-64 shrink-0 flex-col border-r border-white/[0.06] bg-[#1E2128] md:flex">
        <div className="px-6 py-6">
          <span className="font-mono text-sm tracking-[0.25em] text-foreground uppercase">
            {es.app.name}
          </span>
        </div>

        <nav className="flex flex-1 flex-col gap-1 overflow-y-auto px-3">
          {visibleNavItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  'flex items-center gap-3 rounded-lg border-l-2 px-3 py-2 text-sm font-medium transition-colors duration-200',
                  isActive
                    ? 'border-primary bg-primary/10 text-primary'
                    : 'border-transparent text-[#868C97] hover:bg-white/5',
                )
              }
            >
              <item.icon className="size-5 shrink-0" strokeWidth={ICON_STROKE_WIDTH} />
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-white/[0.06] px-3 py-4">
          <div className="flex items-center gap-3 px-3">
            <div className="flex size-9 shrink-0 items-center justify-center rounded-full bg-primary/20 text-sm font-semibold text-primary">
              {getInitials(username ?? '?')}
            </div>
            <div className="min-w-0 flex-1">
              <p className="truncate text-sm font-medium">{username}</p>
              <p className="truncate text-xs text-[#868C97]">{es.auth.tagline}</p>
            </div>
            <ThemeToggle />
            <Button
              variant="ghost"
              size="icon"
              onClick={handleLogout}
              aria-label={es.common.logout}
              title={es.common.logout}
            >
              <LogOut className="size-5" strokeWidth={ICON_STROKE_WIDTH} />
            </Button>
          </div>
        </div>
      </aside>

      <div className="flex min-w-0 flex-1 flex-col">
        {/* Top bar: solo en mobile */}
        <header className="sticky top-0 z-40 border-b border-border bg-background/80 backdrop-blur-lg md:hidden">
          <div className="h-safe-top" aria-hidden="true" />
          <div className="flex h-14 items-center justify-between px-4">
            <span className="font-heading text-xl font-bold tracking-tight text-foreground">
              {es.app.name}
            </span>
            <div className="flex size-9 shrink-0 items-center justify-center rounded-full border border-border bg-primary/15 text-sm font-semibold text-primary">
              {getInitials(username ?? '?')}
            </div>
          </div>
        </header>

        <main className="min-w-0 flex-1 overflow-x-hidden px-4 py-6 pb-[calc(5rem+env(safe-area-inset-bottom))] sm:px-6 md:pb-6">
          <div className="mx-auto max-w-6xl">
            <Outlet />
          </div>
        </main>

        {/* Barra de navegación inferior: solo en mobile */}
        <nav
          className="fixed inset-x-0 bottom-0 z-40 border-t border-border bg-background/85 backdrop-blur-lg md:hidden"
          aria-label={es.app.name}
        >
          <div className="flex items-stretch justify-around px-1 pt-1">
            {mobilePrimaryItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  cn(
                    'group flex flex-1 flex-col items-center gap-1 py-1.5 text-muted-foreground transition-colors',
                    isActive && 'text-primary',
                  )
                }
              >
                {({ isActive }) => (
                  <>
                    <span
                      className={cn(
                        'flex h-8 w-14 items-center justify-center rounded-full transition-all duration-200 group-active:scale-90',
                        isActive && 'bg-primary/12',
                      )}
                    >
                      <item.icon className="size-5" strokeWidth={ICON_STROKE_WIDTH} aria-hidden="true" />
                    </span>
                    <span className="text-[0.6875rem] font-medium leading-none">{item.label}</span>
                  </>
                )}
              </NavLink>
            ))}

            {mobileMoreItems.length > 0 && (
              <Sheet open={moreOpen} onOpenChange={setMoreOpen}>
                <SheetTrigger asChild>
                  <button
                    type="button"
                    aria-label={es.nav.more}
                    className={cn(
                      'group flex flex-1 flex-col items-center gap-1 py-1.5 text-muted-foreground transition-colors',
                      (moreActive || moreOpen) && 'text-primary',
                    )}
                  >
                    <span
                      className={cn(
                        'flex h-8 w-14 items-center justify-center rounded-full transition-all duration-200 group-active:scale-90',
                        (moreActive || moreOpen) && 'bg-primary/12',
                      )}
                    >
                      <MoreHorizontal className="size-5" strokeWidth={ICON_STROKE_WIDTH} aria-hidden="true" />
                    </span>
                    <span className="text-[0.6875rem] font-medium leading-none">{es.nav.more}</span>
                  </button>
                </SheetTrigger>

                <SheetContent side="bottom" className="rounded-t-3xl pb-safe">
                  <div className="mx-auto mt-2 h-1.5 w-10 rounded-full bg-border" aria-hidden="true" />
                  <SheetHeader className="text-left">
                    <SheetTitle>{es.nav.more}</SheetTitle>
                  </SheetHeader>

                  <div className="grid grid-cols-3 gap-2 px-4">
                    {mobileMoreItems.map((item) => (
                      <NavLink
                        key={item.to}
                        to={item.to}
                        onClick={() => setMoreOpen(false)}
                        className={({ isActive }) =>
                          cn(
                            'flex flex-col items-center gap-2 rounded-2xl border border-border bg-background p-4 text-center transition-all active:scale-95 active:bg-accent',
                            isActive && 'border-primary/40 bg-primary/10 text-primary',
                          )
                        }
                      >
                        {({ isActive }) => (
                          <>
                            <item.icon
                              className={cn('size-6', !isActive && 'text-muted-foreground')}
                              strokeWidth={ICON_STROKE_WIDTH}
                              aria-hidden="true"
                            />
                            <span className="text-xs font-medium leading-tight">{item.label}</span>
                          </>
                        )}
                      </NavLink>
                    ))}
                  </div>

                  <div className="mt-4 flex flex-col gap-1 border-t border-border px-4 pt-4 pb-4">
                    <button
                      type="button"
                      onClick={toggleTheme}
                      className="flex items-center gap-3 rounded-xl px-3 py-3 text-sm font-medium text-foreground transition-colors active:bg-accent"
                    >
                      {theme === 'dark' ? (
                        <Sun className="size-5 text-muted-foreground" strokeWidth={ICON_STROKE_WIDTH} aria-hidden="true" />
                      ) : (
                        <Moon className="size-5 text-muted-foreground" strokeWidth={ICON_STROKE_WIDTH} aria-hidden="true" />
                      )}
                      {theme === 'dark' ? es.theme.toggleToLight : es.theme.toggleToDark}
                    </button>
                    <button
                      type="button"
                      onClick={() => {
                        setMoreOpen(false)
                        handleLogout()
                      }}
                      className="flex items-center gap-3 rounded-xl px-3 py-3 text-sm font-medium text-destructive transition-colors active:bg-destructive/10"
                    >
                      <LogOut className="size-5" strokeWidth={ICON_STROKE_WIDTH} aria-hidden="true" />
                      {es.common.logout}
                    </button>
                  </div>
                </SheetContent>
              </Sheet>
            )}
          </div>

          <div className="pb-safe" aria-hidden="true" />
        </nav>
      </div>
    </div>
  )
}
