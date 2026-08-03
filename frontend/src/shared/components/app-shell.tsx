import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { LogOut } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useSessionStore } from '@/shared/store/session-store'
import { Button } from '@/shared/components/ui/button'
import { ThemeToggle } from '@/shared/components/theme-toggle'
import { cn } from '@/shared/lib/utils'

const navItems = [
  { to: '/today', label: es.nav.today },
  { to: '/finance', label: es.nav.finance },
  { to: '/training', label: es.nav.training },
  { to: '/tasks', label: es.nav.tasks },
  { to: '/goals', label: es.nav.goals },
  { to: '/projects', label: es.nav.projects },
  { to: '/subscriptions', label: es.nav.subscriptions },
]

export function AppShell() {
  const navigate = useNavigate()
  const clearSession = useSessionStore((state) => state.clearSession)

  const handleLogout = () => {
    clearSession()
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="border-b border-border">
        <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-3 sm:px-6">
          <span className="font-heading text-lg font-semibold tracking-tight">{es.app.name}</span>
          <div className="flex items-center gap-1">
            <ThemeToggle />
            <Button
              variant="ghost"
              size="icon"
              onClick={handleLogout}
              aria-label={es.common.logout}
              title={es.common.logout}
            >
              <LogOut className="size-5" />
            </Button>
          </div>
        </div>
        <nav className="mx-auto flex max-w-6xl gap-1 overflow-x-auto px-4 pb-2 sm:px-6">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                cn(
                  'shrink-0 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-primary text-primary-foreground'
                    : 'text-muted-foreground hover:bg-muted hover:text-foreground',
                )
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-6 sm:px-6">
        <Outlet />
      </main>
    </div>
  )
}
