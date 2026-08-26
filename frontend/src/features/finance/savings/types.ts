export interface SavingsGoal {
  id: number
  name: string
  targetAmount: number
  currentAmount: number
  dueDate: string | null
  monthlyContributionTarget: number | null
  currentStreakMonths: number
  longestStreakMonths: number
}

export interface SavingsGoalFormValues {
  name: string
  targetAmount: number
  currentAmount: number
  dueDate: string | null
  monthlyContributionTarget: number | null
}

export interface ContributeSavingsGoalValues {
  amount: number
}

// Configuración financiera personal (ingreso mensual), independiente de cualquier
// meta puntual. Hay un solo valor por usuario, no una lista.
export interface FinancialProfile {
  monthlyIncome: number
}
