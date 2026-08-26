export type AchievementCategory = 'finanzas' | 'entrenamiento'

export interface Achievement {
  key: string
  category: AchievementCategory
  title: string
  description: string
  unlocked: boolean
  unlockedAt: string | null
}
