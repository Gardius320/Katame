import type { TaskItem } from '@/features/tasks/types'
import type { DayOfWeek, Exercise } from '@/features/training/types'

export type UpcomingDueType = 'obligation' | 'credit_card' | 'subscription'

export interface UpcomingDue {
  type: UpcomingDueType
  name: string
  dueDate: string
  amount: number | null
}

export interface BalanceTrendPoint {
  date: string
  amount: number
}

export interface TodayTraining {
  id: number
  dayOfWeek: DayOfWeek
  title: string
  exercises: Exercise[]
}

export interface Today {
  balance: number
  balanceTrend: BalanceTrendPoint[]
  upcomingDueDates: UpcomingDue[]
  todayTraining: TodayTraining | null
  urgentTasks: TaskItem[]
}
