export interface Goal {
  id: number
  title: string
  category: string
  progressPercentage: number
  dueDate: string | null
}

export interface GoalFormValues {
  title: string
  category: string
  progressPercentage: number
  dueDate: string | null
}
