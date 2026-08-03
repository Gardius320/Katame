import { useEffect } from 'react'
import { useUiStore } from '@/shared/store/ui-store'

export function useThemeSync() {
  const theme = useUiStore((state) => state.theme)

  useEffect(() => {
    const root = document.documentElement
    root.classList.toggle('dark', theme === 'dark')
    root.classList.toggle('light', theme === 'light')
  }, [theme])
}
