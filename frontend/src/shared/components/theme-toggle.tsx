import { Moon, Sun } from 'lucide-react'
import { Button } from '@/shared/components/ui/button'
import { useUiStore } from '@/shared/store/ui-store'
import { es } from '@/shared/i18n/es'

export function ThemeToggle() {
  const theme = useUiStore((state) => state.theme)
  const toggleTheme = useUiStore((state) => state.toggleTheme)
  const isDark = theme === 'dark'

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={toggleTheme}
      aria-label={isDark ? es.theme.toggleToLight : es.theme.toggleToDark}
      title={isDark ? es.theme.toggleToLight : es.theme.toggleToDark}
    >
      {isDark ? <Sun className="size-5" /> : <Moon className="size-5" />}
    </Button>
  )
}
