import { useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import { useDeleteSavingsGoal, useSavingsGoals } from './hooks'
import { SavingsFormDialog } from './savings-form-dialog'
import type { SavingsGoal } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
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

export default function SavingsPage() {
  const { data: goals, isLoading } = useSavingsGoals()
  const deleteGoal = useDeleteSavingsGoal()

  const [formOpen, setFormOpen] = useState(false)
  const [editingGoal, setEditingGoal] = useState<SavingsGoal | null>(null)
  const [goalToDelete, setGoalToDelete] = useState<SavingsGoal | null>(null)

  const openCreateForm = () => {
    setEditingGoal(null)
    setFormOpen(true)
  }

  const openEditForm = (goal: SavingsGoal) => {
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
          <h1 className="font-heading text-2xl font-semibold">{es.finance.savings.title}</h1>
          <p className="text-muted-foreground">{es.finance.savings.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.finance.savings.newGoal}
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
            const progress = goal.targetAmount > 0 ? goal.currentAmount / goal.targetAmount : 0
            const percent = Math.min(100, Math.round(progress * 100))
            const isComplete = progress >= 1

            return (
              <Card key={goal.id} className="gap-3 p-4">
                <div className="flex items-start justify-between gap-2">
                  <p className="font-heading font-semibold">{goal.name}</p>
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

                <p className="font-numeric text-sm text-muted-foreground">
                  {es.finance.savings.progressLabel
                    .replace('{current}', formatCurrency(goal.currentAmount))
                    .replace('{target}', formatCurrency(goal.targetAmount))}
                </p>
              </Card>
            )
          })}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">
          {es.finance.savings.emptyState}
        </Card>
      )}

      <SavingsFormDialog open={formOpen} onOpenChange={setFormOpen} goal={editingGoal} />

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
