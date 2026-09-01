import { useEffect, useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import { useEvaluateAchievements } from '@/features/achievements/hooks'
import { useBudgets, useDeleteBudget } from './hooks'
import { AntExpensesCard } from './ant-expenses-card'
import { BudgetFormDialog } from './budget-form-dialog'
import type { Budget } from './types'
import { AchievementUnlockedDialog } from '@/shared/components/achievement-unlocked-dialog'
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

const resetDateFormatter = new Intl.DateTimeFormat('es-CO', { day: 'numeric', month: 'short' })

function formatResetDate(cycleEnd: string): string {
  return resetDateFormatter.format(new Date(cycleEnd))
}

export default function BudgetsPage() {
  const { data: budgets, isLoading } = useBudgets()
  const deleteBudget = useDeleteBudget()
  const evaluateAchievements = useEvaluateAchievements()

  const [formOpen, setFormOpen] = useState(false)
  const [editingBudget, setEditingBudget] = useState<Budget | null>(null)
  const [budgetToDelete, setBudgetToDelete] = useState<Budget | null>(null)
  const [celebration, setCelebration] = useState<{ open: boolean; title: string; description: string }>({
    open: false,
    title: '',
    description: '',
  })

  // Presupuestos es donde se ve el aviso de gastos hormiga, así que también
  // es un buen momento para revisar si eso desbloqueó "mes sin gastos
  // hormiga" (ver AchievementService.IsAntExpenseFreeLastMonthAsync).
  useEffect(() => {
    evaluateAchievements.mutate(undefined, {
      onSuccess: (newlyUnlocked) => {
        const [first] = newlyUnlocked
        if (first) {
          setCelebration({ open: true, title: first.title, description: first.description })
        }
      },
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const openCreateForm = () => {
    setEditingBudget(null)
    setFormOpen(true)
  }

  const openEditForm = (budget: Budget) => {
    setEditingBudget(budget)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!budgetToDelete) return
    deleteBudget.mutate(budgetToDelete.id, { onSuccess: () => setBudgetToDelete(null) })
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.finance.budgets.title}</h1>
          <p className="text-muted-foreground">{es.finance.budgets.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.finance.budgets.newBudget}
        </Button>
      </div>

      <AntExpensesCard />

      {isLoading ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 2 }).map((_, index) => (
            <Skeleton key={index} className="h-32 w-full rounded-xl" />
          ))}
        </div>
      ) : budgets && budgets.length > 0 ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {budgets.map((budget) => {
            const percent = budget.amount > 0 ? Math.min(100, (budget.spent / budget.amount) * 100) : 0
            const isOverLimit = budget.spent > budget.amount
            const isNearLimit = !isOverLimit && percent >= 80

            return (
              <Card key={budget.id} className="min-w-0 gap-3 p-4">
                <div className="flex items-start justify-between gap-2">
                  <div>
                    <p className="font-heading font-semibold">{budget.category}</p>
                    <p className="text-xs text-muted-foreground">
                      {es.finance.budgets.period[budget.period]}
                    </p>
                  </div>
                  <div className="flex shrink-0 items-center gap-1">
                    <Button
                      variant="ghost"
                      size="icon-sm"
                      aria-label={es.common.edit}
                      title={es.common.edit}
                      onClick={() => openEditForm(budget)}
                    >
                      <Pencil className="size-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon-sm"
                      aria-label={es.common.delete}
                      title={es.common.delete}
                      onClick={() => setBudgetToDelete(budget)}
                    >
                      <Trash2 className="size-4" />
                    </Button>
                  </div>
                </div>

                <div className="h-2 w-full overflow-hidden rounded-full bg-muted">
                  <div
                    className={cn(
                      'h-full rounded-full transition-all',
                      isOverLimit ? 'bg-destructive' : isNearLimit ? 'bg-amber-400' : 'bg-primary',
                    )}
                    style={{ width: `${percent}%` }}
                  />
                </div>

                <p
                  className={cn(
                    'font-numeric text-sm',
                    isOverLimit ? 'font-semibold text-destructive' : 'text-muted-foreground',
                  )}
                >
                  {es.finance.budgets.progressLabel
                    .replace('{spent}', formatCurrency(budget.spent))
                    .replace('{amount}', formatCurrency(budget.amount))}
                </p>

                <p className="text-xs text-muted-foreground">
                  {es.finance.budgets.resetsLabel.replace('{date}', formatResetDate(budget.cycleEnd))}
                </p>

                {isOverLimit && (
                  <p className="text-xs font-medium text-destructive">
                    {es.finance.budgets.overLimitLabel}
                  </p>
                )}
              </Card>
            )
          })}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">
          {es.finance.budgets.emptyState}
        </Card>
      )}

      <BudgetFormDialog open={formOpen} onOpenChange={setFormOpen} budget={editingBudget} />

      <AchievementUnlockedDialog
        open={celebration.open}
        onOpenChange={(open) => setCelebration((prev) => ({ ...prev, open }))}
        title={celebration.title}
        description={celebration.description}
      />

      <AlertDialog
        open={budgetToDelete !== null}
        onOpenChange={(open) => !open && setBudgetToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteBudget.isPending}>
              {deleteBudget.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
