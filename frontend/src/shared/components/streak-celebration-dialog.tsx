import { useEffect } from 'react'
import { Flame } from 'lucide-react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
} from '@/shared/components/ui/dialog'

interface StreakCelebrationDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  streakCount: number
  title: string
  description: string
}

// Exportado para que las pantallas que también puedan mostrar un logro justo
// después de esta racha (Ahorros, Entrenamiento) sepan cuánto esperar antes
// de abrir ese segundo modal, en vez de superponer los dos.
export const STREAK_CELEBRATION_DURATION_MS = 2600
const AUTO_DISMISS_MS = STREAK_CELEBRATION_DURATION_MS

/**
 * Aviso animado compartido entre Ahorros y Entrenamiento -- mismo ícono de
 * fuego para las dos rachas, solo cambia el número y el texto. Se cierra solo
 * después de un momento (además de poder cerrarse con Escape o haciendo clic
 * afuera), porque es una celebración puntual, no un formulario que esperar.
 */
export function StreakCelebrationDialog({
  open,
  onOpenChange,
  streakCount,
  title,
  description,
}: StreakCelebrationDialogProps) {
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
            <Flame className="size-16 fill-orange-500 text-orange-500" />
          </div>
          <p className="font-numeric text-5xl font-bold">{streakCount}</p>
          <DialogTitle className="font-heading text-lg">{title}</DialogTitle>
          <DialogDescription>{description}</DialogDescription>
        </div>
      </DialogContent>
    </Dialog>
  )
}
