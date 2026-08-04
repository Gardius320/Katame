import { apiClient } from '@/shared/api/client'
import type { SavingsGoal, SavingsGoalFormValues } from './types'

export async function getSavingsGoals(): Promise<SavingsGoal[]> {
  const { data } = await apiClient.get<SavingsGoal[]>('/finance/savings')
  return data
}

export async function createSavingsGoal(payload: SavingsGoalFormValues): Promise<SavingsGoal> {
  const { data } = await apiClient.post<SavingsGoal>('/finance/savings', payload)
  return data
}

export async function updateSavingsGoal(
  id: number,
  payload: SavingsGoalFormValues,
): Promise<SavingsGoal> {
  const { data } = await apiClient.put<SavingsGoal>(`/finance/savings/${id}`, payload)
  return data
}

export async function deleteSavingsGoal(id: number): Promise<void> {
  await apiClient.delete(`/finance/savings/${id}`)
}
