import { apiClient } from '@/shared/api/client'
import type { CreateObligationRequest, Obligation, UpdateObligationRequest } from './types'

export async function getObligations(): Promise<Obligation[]> {
  const { data } = await apiClient.get<Obligation[]>('/finance/obligations')
  return data
}

export async function createObligation(payload: CreateObligationRequest): Promise<Obligation> {
  const { data } = await apiClient.post<Obligation>('/finance/obligations', payload)
  return data
}

export async function updateObligation(
  id: number,
  payload: UpdateObligationRequest,
): Promise<Obligation> {
  const { data } = await apiClient.put<Obligation>(`/finance/obligations/${id}`, payload)
  return data
}

export async function deleteObligation(id: number): Promise<void> {
  await apiClient.delete(`/finance/obligations/${id}`)
}
