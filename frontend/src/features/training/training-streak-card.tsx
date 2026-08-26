import { Flame } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { useMarkTrainingCompleted, useTrainingStreak } from './hooks'
import type { TrainingStreak } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
import { Skeleton } from '@/shared/components/ui/skeleton'

interface TrainingStreakCardProps {
  onCompleted: (streak: TrainingStreak) => void
}

// Tarjeta junto a la meta semanal: muestra la racha de días entrenados
// seguidos y el botón para marcar el día de hoy.
export function TrainingStreakCard({ onCompleted }: TrainingStreakCardProps) {
  const { data: streak, isLoading } = useTrainingStreak()
  const markCompleted = useMarkTrainingCompleted()

  if (isLoading) {
    return <Skeleton className="h-full w-48 rounded-xl" />
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

  return (
    <Card className="flex w-fit flex-col items-center gap-2 p-4">
      <div className="flex items-center gap-1.5">
        <Flame className="size-5 fill-orange-500 text-orange-500" />
        <p className="font-numeric text-2xl font-bold">{streak?.currentStreakDays ?? 0}</p>
      </div>
      <p className="text-center text-xs text-muted-foreground">
        {es.training.streak.currentLabel}
        {streak && streak.longestStreakDays > 0 ? (
          <>
            <br />
            {es.training.streak.longestLabel.replace('{count}', String(streak.longestStreakDays))}
          </>
        ) : null}
      </p>
      <Button size="sm" onClick={handleClick} disabled={markCompleted.isPending}>
        {es.training.streak.markCompletedToday}
      </Button>
    </Card>
  )
}
