export interface SavingsGoal {
  id: number
  name: string
  targetAmount: number
  currentAmount: number
  dueDate: string | null
}

export interface SavingsGoalFormValues {
  name: string
  targetAmount: number
  currentAmount: number
  dueDate: string | null
}
