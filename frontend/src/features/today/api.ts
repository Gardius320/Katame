import { apiClient } from '@/shared/api/client'
import type { Today } from './types'

export async function getToday(): Promise<Today> {
  const { data } = await apiClient.get<Today>('/today')
  return data
}
