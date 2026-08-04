import { apiClient } from '@/shared/api/client'
import type { CreateSubscriptionRequest, Subscription, UpdateSubscriptionRequest } from './types'

export async function getSubscriptions(): Promise<Subscription[]> {
  const { data } = await apiClient.get<Subscription[]>('/subscriptions')
  return data
}

export async function createSubscription(
  payload: CreateSubscriptionRequest,
): Promise<Subscription> {
  const { data } = await apiClient.post<Subscription>('/subscriptions', payload)
  return data
}

export async function updateSubscription(
  id: number,
  payload: UpdateSubscriptionRequest,
): Promise<Subscription> {
  const { data } = await apiClient.put<Subscription>(`/subscriptions/${id}`, payload)
  return data
}

export async function deleteSubscription(id: number): Promise<void> {
  await apiClient.delete(`/subscriptions/${id}`)
}
