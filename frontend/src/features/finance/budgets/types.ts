export type BudgetPeriod = 'weekly' | 'biweekly' | 'monthly'

export interface Budget {
  id: number
  category: string
  amount: number
  period: BudgetPeriod
  anchorDate: string
  /** Inicio del ciclo vigente (inclusive). */
  cycleStart: string
  /** Fecha en la que se reinicia el ciclo (exclusive). */
  cycleEnd: string
  /** Gastado en la categoría durante el ciclo vigente. */
  spent: number
}

export interface BudgetFormValues {
  category: string
  amount: number
  period: BudgetPeriod
  anchorDate: string
}
