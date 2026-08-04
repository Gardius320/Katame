import { apiClient } from '@/shared/api/client'
import type { CreditCard, CreditCardFormValues } from './types'

export async function getCreditCards(): Promise<CreditCard[]> {
  const { data } = await apiClient.get<CreditCard[]>('/finance/credit-cards')
  return data
}

export async function createCreditCard(payload: CreditCardFormValues): Promise<CreditCard> {
  const { data } = await apiClient.post<CreditCard>('/finance/credit-cards', payload)
  return data
}

export async function updateCreditCard(
  id: number,
  payload: CreditCardFormValues,
): Promise<CreditCard> {
  const { data } = await apiClient.put<CreditCard>(`/finance/credit-cards/${id}`, payload)
  return data
}

export async function deleteCreditCard(id: number): Promise<void> {
  await apiClient.delete(`/finance/credit-cards/${id}`)
}
