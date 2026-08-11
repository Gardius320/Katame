import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import {
  CalendarCheck,
  Dumbbell,
  FolderKanban,
  ListTodo,
  LogOut,
  Repeat,
  Target,
  Users,
  Wallet,
  type LucideIcon,
} from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { Button } from '@/shared/components/ui/button'
import { ThemeToggle } from '@/shared/components/theme-toggle'
import { cn } from '@/shared/lib/utils'

const ICON_STROKE_WIDTH = 1.75

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
  const username = useSessionStore((state) => state.username)
  const isAdmin = useSessionStore((state) => state.isAdmin)
  const clearSession = useSessionStore((state) => state.clearSession)

  const handleLogout = () => {
    clearSession()
    navigate('/login', { replace: true })
  }

  const visibleNavItems = isAdmin ? [...navItems, adminNavItem] : navItems

  return (
    <div className="flex min-h-screen bg-background text-foreground">
      <aside className="flex w-64 shrink-0 flex-col border-r border-white/[0.06] bg-[#1E2128]">
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

      <main className="min-w-0 flex-1 overflow-x-hidden px-4 py-6 sm:px-6">
        <div className="mx-auto max-w-6xl">
          <Outlet />
        </div>
      </main>
    </div>
  )
}
