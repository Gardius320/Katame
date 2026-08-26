import { useState } from 'react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { useUpdateSavingsGoal } from './hooks'
import type { SavingsGoal } from './types'
import { Button } from '@/shared/components/ui/button'
import { Slider } from '@/shared/components/ui/slider'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog'

interface GoalProjectionDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  goal: SavingsGoal | null
}

const monthYearFormatter = new Intl.DateTimeFormat('es-CO', { month: 'long', year: 'numeric' })

function projectedDateLabel(monthsFromNow: number): string {
  const date = new Date()
  // Se fija el día en 1 antes de sumar meses para que un "día 31" no se
  // desborde a un mes que no tiene ese día (ej. 31 de enero + 1 mes).
  date.setDate(1)
  date.setMonth(date.getMonth() + monthsFromNow)
  return monthYearFormatter.format(date)
}

/**
 * "¿Cuándo cumplo esta meta?" -- proyección de solo lectura sobre datos que
 * ya tenemos (falta vs. aporte mensual), con un slider para jugar con el
 * "¿y si aporto más?" y, si el número convence, aplicarlo como el nuevo
 * aporte mensual planeado de la meta.
 */
export function GoalProjectionDialog({ open, onOpenChange, goal }: GoalProjectionDialogProps) {
  const updateGoal = useUpdateSavingsGoal()
  const remaining = goal ? Math.max(0, goal.targetAmount - goal.currentAmount) : 0
  const alreadyReached = remaining <= 0

  const defaultMonthly =
    goal?.monthlyContributionTarget && goal.monthlyContributionTarget > 0
      ? goal.monthlyContributionTarget
      : Math.max(1, Math.round(remaining / 12))

  const [monthly, setMonthly] = useState(defaultMonthly)

  // Reinicia el slider al valor por defecto cuando el diálogo se abre (o
  // cambia de meta), sin usar un efecto -- ajustar estado en el cuerpo del
  // render, siguiendo el patrón que recomienda React para esto, evita el
  // re-render en cascada que un setState dentro de un efecto provocaría.
  const [seed, setSeed] = useState({ open, goalId: goal?.id ?? null })
  if (open !== seed.open || (goal?.id ?? null) !== seed.goalId) {
    setSeed({ open, goalId: goal?.id ?? null })
    if (open) {
      setMonthly(defaultMonthly)
    }
  }

  if (!goal) {
    return null
  }

  const sliderMax = Math.max(remaining, defaultMonthly * 3, 100)
  const sliderStep = Math.max(1, Math.round(sliderMax / 100))
  const monthsToGoal = monthly > 0 ? Math.ceil(remaining / monthly) : null

  const handleApply = () => {
    updateGoal.mutate(
      {
        id: goal.id,
        payload: {
          name: goal.name,
          targetAmount: goal.targetAmount,
          currentAmount: goal.currentAmount,
          dueDate: goal.dueDate,
          monthlyContributionTarget: monthly,
        },
      },
      { onSuccess: () => onOpenChange(false) },
    )
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {es.finance.savings.simulator.title.replace('{name}', goal.name)}
          </DialogTitle>
        </DialogHeader>

        {alreadyReached ? (
          <p className="text-sm text-muted-foreground">{es.finance.savings.simulator.alreadyReached}</p>
        ) : (
          <div className="grid gap-5">
            <div className="text-center">
              <p className="font-numeric text-3xl font-bold">{formatCurrency(monthly)}</p>
              <p className="text-xs text-muted-foreground">{es.finance.savings.simulator.monthlyLabel}</p>
            </div>

            <Slider
              value={[monthly]}
              min={1}
              max={sliderMax}
              step={sliderStep}
              onValueChange={([value]) => setMonthly(value)}
            />

            {monthsToGoal !== null && (
              <div className="rounded-lg bg-muted p-3 text-center text-sm">
                {(monthsToGoal === 1
                  ? es.finance.savings.simulator.projectionLabelSingular
                  : es.finance.savings.simulator.projectionLabel
                )
                  .replace('{months}', String(monthsToGoal))
                  .replace('{date}', projectedDateLabel(monthsToGoal))}
              </div>
            )}
          </div>
        )}

        {!alreadyReached && (
          <DialogFooter className="mt-2">
            <Button
              variant="outline"
              className="w-full sm:w-auto"
              onClick={handleApply}
              disabled={updateGoal.isPending}
            >
              {es.finance.savings.simulator.applyButton}
            </Button>
          </DialogFooter>
        )}
      </DialogContent>
    </Dialog>
  )
}
