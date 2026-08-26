import { useState } from 'react'
import { Calculator, Flame, PiggyBank, Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import { StreakCelebrationDialog, STREAK_CELEBRATION_DURATION_MS } from '@/shared/components/streak-celebration-dialog'
import { AchievementUnlockedDialog } from '@/shared/components/achievement-unlocked-dialog'
import { useEvaluateAchievements } from '@/features/achievements/hooks'
import { useDeleteSavingsGoal, useFinancialProfile, useSavingsGoals } from './hooks'
import { GoalProjectionDialog } from './goal-projection-dialog'
import { IncomeSummaryCard } from './income-summary-card'
import { SavingsContributeDialog } from './savings-contribute-dialog'
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
  const { data: financialProfile } = useFinancialProfile()
  const deleteGoal = useDeleteSavingsGoal()
  const monthlyIncome = financialProfile?.monthlyIncome ?? 0

  const [formOpen, setFormOpen] = useState(false)
  const [editingGoal, setEditingGoal] = useState<SavingsGoal | null>(null)
  const [goalToDelete, setGoalToDelete] = useState<SavingsGoal | null>(null)
  const [contributeOpen, setContributeOpen] = useState(false)
  const [goalToContribute, setGoalToContribute] = useState<SavingsGoal | null>(null)
  const [goalToSimulate, setGoalToSimulate] = useState<SavingsGoal | null>(null)
  const [streakDialog, setStreakDialog] = useState<{ open: boolean; goalName: string; streak: number }>({
    open: false,
    goalName: '',
    streak: 0,
  })
  const [achievementDialog, setAchievementDialog] = useState<{ open: boolean; title: string; description: string }>({
    open: false,
    title: '',
    description: '',
  })
  const evaluateAchievements = useEvaluateAchievements()

  const openCreateForm = () => {
    setEditingGoal(null)
    setFormOpen(true)
  }

  const openEditForm = (goal: SavingsGoal) => {
    setEditingGoal(goal)
    setFormOpen(true)
  }

  const openContributeForm = (goal: SavingsGoal) => {
    setGoalToContribute(goal)
    setContributeOpen(true)
  }

  const handleContributed = (goal: SavingsGoal) => {
    const willShowStreak = goal.currentStreakMonths > 0
    if (willShowStreak) {
      setStreakDialog({ open: true, goalName: goal.name, streak: goal.currentStreakMonths })
    }

    evaluateAchievements.mutate(undefined, {
      onSuccess: (newlyUnlocked) => {
        const [first] = newlyUnlocked
        if (!first) return

        // Si ya se va a mostrar la racha, el logro espera a que esa
        // celebración termine en vez de superponerse.
        const delay = willShowStreak ? STREAK_CELEBRATION_DURATION_MS + 200 : 0
        window.setTimeout(() => {
          setAchievementDialog({ open: true, title: first.title, description: first.description })
        }, delay)
      },
    })
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

      <IncomeSummaryCard goals={goals ?? []} />

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
                      aria-label={es.finance.savings.addFunds}
                      title={es.finance.savings.addFunds}
                      onClick={() => openContributeForm(goal)}
                    >
                      <PiggyBank className="size-4" />
                    </Button>
                    {!isComplete && (
                      <Button
                        variant="ghost"
                        size="icon-sm"
                        aria-label={es.finance.savings.simulator.openButton}
                        title={es.finance.savings.simulator.openButton}
                        onClick={() => setGoalToSimulate(goal)}
                      >
                        <Calculator className="size-4" />
                      </Button>
                    )}
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
                    className="h-full rounded-full transition-all"
                    style={{
                      width: `${percent}%`,
                      backgroundColor: `color-mix(in srgb, var(--positive) ${percent}%, var(--primary))`,
                    }}
                  />
                </div>

                <div className="flex items-center justify-between gap-2">
                  <p className="font-numeric text-sm text-muted-foreground">
                    {es.finance.savings.progressLabel
                      .replace('{current}', formatCurrency(goal.currentAmount))
                      .replace('{target}', formatCurrency(goal.targetAmount))}
                  </p>
                  {isComplete ? (
                    <span className="shrink-0 text-xs font-semibold text-positive">
                      {es.finance.savings.goalReached}
                    </span>
                  ) : percent >= 80 ? (
                    <span className="shrink-0 text-xs font-semibold text-primary">
                      {es.finance.savings.almostThere}
                    </span>
                  ) : null}
                </div>

                {goal.monthlyContributionTarget ? (
                  <p className="font-numeric text-xs text-muted-foreground">
                    {monthlyIncome > 0
                      ? es.finance.savings.perGoalMonthly
                          .replace('{amount}', formatCurrency(goal.monthlyContributionTarget))
                          .replace(
                            '{percent}',
                            String(
                              Math.round((goal.monthlyContributionTarget / monthlyIncome) * 100),
                            ),
                          )
                      : es.finance.savings.perGoalMonthlyNoIncome.replace(
                          '{amount}',
                          formatCurrency(goal.monthlyContributionTarget),
                        )}
                  </p>
                ) : null}

                {goal.currentStreakMonths > 0 ? (
                  <p className="flex items-center gap-1 text-xs font-semibold text-orange-500">
                    <Flame className="size-3.5 fill-orange-500" />
                    {(goal.currentStreakMonths === 1
                      ? es.finance.savings.streakLabelSingular
                      : es.finance.savings.streakLabel
                    ).replace('{count}', String(goal.currentStreakMonths))}
                  </p>
                ) : null}
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

      <SavingsContributeDialog
        open={contributeOpen}
        onOpenChange={setContributeOpen}
        goal={goalToContribute}
        onContributed={handleContributed}
      />

      <GoalProjectionDialog
        open={goalToSimulate !== null}
        onOpenChange={(open) => !open && setGoalToSimulate(null)}
        goal={goalToSimulate}
      />

      <AchievementUnlockedDialog
        open={achievementDialog.open}
        onOpenChange={(open) => setAchievementDialog((prev) => ({ ...prev, open }))}
        title={achievementDialog.title}
        description={achievementDialog.description}
      />

      <StreakCelebrationDialog
        open={streakDialog.open}
        onOpenChange={(open) => setStreakDialog((prev) => ({ ...prev, open }))}
        streakCount={streakDialog.streak}
        title={es.finance.savings.streakDialogTitle}
        description={es.finance.savings.streakDialogDescription.replace(
          '{name}',
          streakDialog.goalName,
        )}
      />

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
