import { apiClient } from '@/shared/api/client'
import type { Achievement } from './types'

export async function getAchievements(): Promise<Achievement[]> {
  const { data } = await apiClient.get<Achievement[]>('/achievements')
  return data
}

// Revisa el catálogo contra el estado actual y desbloquea lo que
// corresponda. Devuelve solo lo que se desbloqueó recién en esta llamada.
export async function evaluateAchievements(): Promise<Achievement[]> {
  const { data } = await apiClient.post<Achievement[]>('/achievements/evaluate')
  return data
}
