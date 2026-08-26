import { useEffect, useState } from 'react'
import { Trophy } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { cn } from '@/shared/lib/utils'
import { AchievementUnlockedDialog } from '@/shared/components/achievement-unlocked-dialog'
import { useAchievements, useEvaluateAchievements } from './hooks'
import type { Achievement, AchievementCategory } from './types'
import { Card } from '@/shared/components/ui/card'
import { Skeleton } from '@/shared/components/ui/skeleton'

const unlockedAtFormatter = new Intl.DateTimeFormat('es-CO', { day: 'numeric', month: 'long', year: 'numeric' })

const CATEGORIES: AchievementCategory[] = ['finanzas', 'entrenamiento']

function AchievementCard({ achievement }: { achievement: Achievement }) {
  return (
    <Card
      className={cn(
        'flex flex-row items-start gap-3 p-4',
        !achievement.unlocked && 'opacity-60',
      )}
    >
      <Trophy
        className={cn(
          'size-8 shrink-0',
          achievement.unlocked ? 'fill-amber-400 text-amber-500' : 'text-muted-foreground',
        )}
      />
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
        ) : null}
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

  // Al entrar a la pantalla, revisa si algo se cumplió pero todavía no se
  // había desbloqueado (ej. el logro "mes sin gastos hormiga", que solo se
  // evalúa de forma perezosa cuando el usuario dispara alguna acción).
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

  return (
    <div className="grid gap-6">
      <div>
        <h1 className="font-heading text-2xl font-semibold">{es.achievements.title}</h1>
        <p className="text-muted-foreground">{es.achievements.subtitle}</p>
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

            return (
              <div key={category} className="grid gap-3">
                <h2 className="font-heading text-lg font-semibold">
                  {es.achievements.categories[category]}
                </h2>
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
