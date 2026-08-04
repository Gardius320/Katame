import { useState } from 'react'
import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { cn } from '@/shared/lib/utils'
import { useDeleteGoal, useGoals } from './hooks'
import { GoalFormDialog } from './goal-form-dialog'
import type { Goal } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Skeleton } from '@/shared/components/ui/skeleton'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog'

function formatDueDate(date: string | null): string | null {
  if (!date) return null
  return format(new Date(date), "d 'de' MMMM", { locale: dateFnsEs })
}

export default function GoalsPage() {
  const { data: goals, isLoading } = useGoals()
  const deleteGoal = useDeleteGoal()

  const [formOpen, setFormOpen] = useState(false)
  const [editingGoal, setEditingGoal] = useState<Goal | null>(null)
  const [goalToDelete, setGoalToDelete] = useState<Goal | null>(null)

  const openCreateForm = () => {
    setEditingGoal(null)
    setFormOpen(true)
  }

  const openEditForm = (goal: Goal) => {
    setEditingGoal(goal)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!goalToDelete) return
    deleteGoal.mutate(goalToDelete.id, { onSuccess: () => setGoalToDelete(null) })
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.goals.title}</h1>
          <p className="text-muted-foreground">{es.goals.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.goals.newGoal}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 2 }).map((_, index) => (
            <Skeleton key={index} className="h-32 w-full rounded-xl" />
          ))}
        </div>
      ) : goals && goals.length > 0 ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {goals.map((goal) => {
            const percent = Math.min(100, Math.max(0, goal.progressPercentage))
            const isComplete = percent >= 100
            const formattedDueDate = formatDueDate(goal.dueDate)

            return (
              <Card key={goal.id} className="gap-3 p-4">
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0">
                    <p className="truncate font-heading font-semibold">{goal.title}</p>
                    <Badge variant="outline" className="mt-1">
                      {goal.category}
                    </Badge>
                  </div>
                  <div className="flex shrink-0 items-center gap-1">
                    <Button
                      variant="ghost"
                      size="icon-sm"
                      aria-label={es.common.edit}
                      title={es.common.edit}
                      onClick={() => openEditForm(goal)}
                    >
                      <Pencil className="size-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon-sm"
                      aria-label={es.common.delete}
                      title={es.common.delete}
                      onClick={() => setGoalToDelete(goal)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                </div>

                <div
                  className={cn(
                    'h-2 w-full overflow-hidden rounded-full bg-muted',
                    isComplete && 'katame-seal',
                  )}
                >
                  <div
                    className={cn(
                      'h-full rounded-full transition-all',
                      isComplete ? 'bg-positive' : 'bg-primary',
                    )}
                    style={{ width: `${percent}%` }}
                  />
                </div>

                <div className="flex items-center justify-between">
                  <p className="font-numeric text-sm text-muted-foreground">
                    {es.goals.progressLabel.replace('{percent}', String(percent))}
                  </p>
                  {formattedDueDate && (
                    <p className="font-numeric text-xs text-muted-foreground">
                      {formattedDueDate}
                    </p>
                  )}
                </div>
              </Card>
            )
          })}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">{es.goals.emptyState}</Card>
      )}

      <GoalFormDialog open={formOpen} onOpenChange={setFormOpen} goal={editingGoal} />

      <AlertDialog
        open={goalToDelete !== null}
        onOpenChange={(open) => !open && setGoalToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteGoal.isPending}>
              {deleteGoal.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
