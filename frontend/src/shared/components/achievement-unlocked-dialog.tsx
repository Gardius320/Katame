import { useEffect } from 'react'
import { Trophy } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from '@/shared/components/ui/dialog'

interface AchievementUnlockedDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  title: string
  description: string
}

const AUTO_DISMISS_MS = 3200

/**
 * Aviso animado para cuando se desbloquea un logro nuevo (financiero o de
 * entrenamiento) -- mismo patrón que StreakCelebrationDialog, reutilizando
 * la animación katame-flame para el ícono, pero con un trofeo en vez de fuego
 * ya que un logro es un hito puntual, no un contador que sigue subiendo.
 */
export function AchievementUnlockedDialog({
  open,
  onOpenChange,
  title,
  description,
}: AchievementUnlockedDialogProps) {
  useEffect(() => {
    if (!open) return
    const timer = setTimeout(() => onOpenChange(false), AUTO_DISMISS_MS)
    return () => clearTimeout(timer)
  }, [open, onOpenChange])

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-sm" showCloseButton={false}>
        <div className="flex flex-col items-center gap-2 py-2 text-center">
          <div className="katame-flame katame-flame-flicker">
            <Trophy className="size-16 fill-amber-400 text-amber-500" />
          </div>
          <p className="text-xs font-semibold tracking-wide text-amber-500 uppercase">
            {es.achievements.unlockedLabel}
          </p>
          <DialogTitle className="font-heading text-lg">{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </div>
      </DialogContent>
    </Dialog>
  )
}
