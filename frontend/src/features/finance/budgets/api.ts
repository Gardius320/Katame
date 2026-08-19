import { apiClient } from '@/shared/api/client'
import type { Budget, BudgetFormValues } from './types'

export async function getBudgets(): Promise<Budget[]> {
  const { data } = await apiClient.get<Budget[]>('/finance/budgets')
  return data
}

export async function createBudget(payload: BudgetFormValues): Promise<Budget> {
  const { data } = await apiClient.post<Budget>('/finance/budgets', payload)
  return data
}

export async function updateBudget(id: number, payload: BudgetFormValues): Promise<Budget> {
  const { data } = await apiClient.put<Budget>(`/finance/budgets/${id}`, payload)
  return data
}

export async function deleteBudget(id: number): Promise<void> {
  await apiClient.delete(`/finance/budgets/${id}`)
}
