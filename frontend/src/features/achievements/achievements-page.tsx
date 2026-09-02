import { useEffect, useState } from 'react'
import { Dumbbell, Lock, Trophy, Wallet } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { cn } from '@/shared/lib/utils'
import { AchievementUnlockedDialog } from '@/shared/components/achievement-unlocked-dialog'
import { useAchievements, useEvaluateAchievements } from './hooks'
import type { Achievement, AchievementCategory } from './types'
import { Card } from '@/shared/components/ui/card'
import { Skeleton } from '@/shared/components/ui/skeleton'

const unlockedAtFormatter = new Intl.DateTimeFormat('es-CO', { day: 'numeric', month: 'long', year: 'numeric' })

const CATEGORIES: AchievementCategory[] = ['finanzas', 'entrenamiento']

const CATEGORY_ICON: Record<AchievementCategory, typeof Wallet> = {
  finanzas: Wallet,
  entrenamiento: Dumbbell,
}

function AchievementCard({ achievement }: { achievement: Achievement }) {
  return (
    <Card
      className={cn(
        'flex flex-row items-start gap-3 p-4',
        !achievement.unlocked && 'bg-muted/40 opacity-70',
      )}
    >
      <div
        className={cn(
          'flex size-11 shrink-0 items-center justify-center rounded-full',
          achievement.unlocked ? 'bg-amber-400/15' : 'bg-muted',
        )}
      >
        {achievement.unlocked ? (
          <Trophy className="size-5 fill-amber-400 text-amber-500" />
        ) : (
          <Lock className="size-4 text-muted-foreground" />
        )}
      </div>
      <div className="min-w-0">
        <p className="font-heading font-semibold">{achievement.title}</p>
        <p className="text-sm text-muted-foreground">{achievement.description}</p>
        {achievement.unlocked && achievement.unlockedAt ? (
          <p className="mt-1 text-xs font-medium text-amber-500">
            {es.achievements.unlockedOn.replace(
              '{date}',
              unlockedAtFormatter.format(new Date(achievement.unlockedAt)),
            )}
          </p>
        ) : (
          <p className="mt-1 text-xs font-medium text-muted-foreground">
            {es.achievements.lockedLabel}
          </p>
        )}
      </div>
    </Card>
  )
}

export default function AchievementsPage() {
  const { data: achievements, isLoading } = useAchievements()
  const evaluate = useEvaluateAchievements()
  const [celebration, setCelebration] = useState<{ open: boolean; title: string; description: string }>({
    open: false,
    title: '',
    description: '',
  })

  // Al entrar a la pantalla, revisa si algo se cumplio pero todavia no se
  // habia desbloqueado (ej. el logro "mes sin gastos hormiga", que solo se
  // evalua de forma perezosa cuando el usuario dispara alguna accion).
  useEffect(() => {
    evaluate.mutate(undefined, {
      onSuccess: (newlyUnlocked) => {
        const [first] = newlyUnlocked
        if (first) {
          setCelebration({ open: true, title: first.title, description: first.description })
        }
      },
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const total = achievements?.length ?? 0
  const unlockedCount = achievements?.filter((a) => a.unlocked).length ?? 0
  const progressPercent = total > 0 ? Math.round((unlockedCount / total) * 100) : 0

  return (
    <div className="grid gap-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between sm:gap-4">
        <div className="min-w-0">
          <h1 className="font-heading text-2xl font-semibold">{es.achievements.title}</h1>
          <p className="text-muted-foreground">{es.achievements.subtitle}</p>
        </div>
        {!isLoading && total > 0 && (
          <div className="flex flex-col gap-1.5 sm:w-48 sm:shrink-0">
            <p className="text-right font-numeric text-xs font-medium text-muted-foreground">
              {es.achievements.progressLabel
                .replace('{unlocked}', String(unlockedCount))
                .replace('{total}', String(total))}
            </p>
            <div className="h-2 w-full overflow-hidden rounded-full bg-muted">
              <div
                className="h-full rounded-full bg-gradient-to-r from-amber-400 to-amber-500 transition-[width] duration-500 ease-out"
                style={{ width: `${progressPercent}%` }}
              />
            </div>
          </div>
        )}
      </div>

      {isLoading ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 4 }).map((_, index) => (
            <Skeleton key={index} className="h-24 w-full rounded-xl" />
          ))}
        </div>
      ) : (
        <div className="grid gap-8">
          {CATEGORIES.map((category) => {
            const items = (achievements ?? []).filter((a) => a.category === category)
            if (items.length === 0) return null

            const CategoryIcon = CATEGORY_ICON[category]
            const categoryUnlocked = items.filter((a) => a.unlocked).length

            return (
              <div key={category} className="grid gap-3">
                <div className="flex items-center gap-2">
                  <CategoryIcon className="size-4 text-muted-foreground" />
                  <h2 className="font-heading text-lg font-semibold">
                    {es.achievements.categories[category]}
                  </h2>
                  <span className="font-numeric text-sm text-muted-foreground">
                    {categoryUnlocked}/{items.length}
                  </span>
                </div>
                <div className="grid gap-3 sm:grid-cols-2">
                  {items.map((achievement) => (
                    <AchievementCard key={achievement.key} achievement={achievement} />
                  ))}
                </div>
              </div>
            )
          })}
        </div>
      )}

      <AchievementUnlockedDialog
        open={celebration.open}
        onOpenChange={(open) => setCelebration((prev) => ({ ...prev, open }))}
        title={celebration.title}
        description={celebration.description}
      />
    </div>
  )
}
