import { Flame } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useMarkTrainingCompleted, useTrainingStreak } from './hooks'
import type { TrainingStreak } from './types'
import { Button } from '@/shared/components/ui/button'
import { Skeleton } from '@/shared/components/ui/skeleton'

interface TrainingStreakCardProps {
  onCompleted: (streak: TrainingStreak) => void
}

// Tarjeta junto a la meta semanal: muestra la racha de días entrenados
// seguidos y el botón para marcar el día de hoy. Usa el mismo lenguaje
// visual "hero" (gradiente + sombra de color) que las StatCard de Finanzas,
// pero en tonos cálidos para que la racha se sienta como una llama viva.
export function TrainingStreakCard({ onCompleted }: TrainingStreakCardProps) {
  const { data: streak, isLoading } = useTrainingStreak()
  const markCompleted = useMarkTrainingCompleted()

  if (isLoading) {
    return <Skeleton className="h-full min-h-[168px] w-full min-w-[220px] rounded-2xl sm:w-64" />
  }

  const handleClick = () => {
    markCompleted.mutate(undefined, {
      onSuccess: (data) => {
        if (data.isNewCompletion) {
          onCompleted(data)
        }
      },
    })
  }

  const currentDays = streak?.currentStreakDays ?? 0
  const hasStreak = currentDays > 0

  return (
    <div
      data-slot="streak-card"
      className="relative flex min-w-[220px] flex-1 flex-col gap-3 rounded-2xl border border-transparent bg-gradient-to-br from-[#FF9A56] via-[#FF6B5C] to-[#EF4444] p-5 shadow-[0_8px_32px_-8px_rgba(255,107,92,0.45)] sm:max-w-64"
    >
      <div className="flex items-center gap-3">
        <div className="flex size-10 shrink-0 items-center justify-center rounded-full bg-white/20 text-white">
          <Flame className="size-5 fill-white" />
        </div>
        <p className="font-mono text-xs tracking-wide text-white/90 uppercase">
          {hasStreak ? es.training.streak.currentLabel : es.training.streak.currentLabelZero}
        </p>
      </div>

      <div className="flex items-end gap-2">
        <p className="font-numeric text-4xl font-bold text-white">{currentDays}</p>
        {hasStreak && (
          <p className="pb-1 text-sm font-medium text-white/80">
            {currentDays === 1 ? 'día' : 'días'}
          </p>
        )}
      </div>

      <p className="text-xs text-white/80">
        {streak && streak.longestStreakDays > 0
          ? es.training.streak.longestLabel.replace('{count}', String(streak.longestStreakDays))
          : es.training.streak.explainer}
      </p>

      <Button
        size="sm"
        className="mt-1 w-full bg-white text-[#EF4444] hover:bg-white/90"
        onClick={handleClick}
        disabled={markCompleted.isPending}
      >
        {es.training.streak.markCompletedToday}
      </Button>
    </div>
  )
}
