import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import {
  createSubscription,
  deleteSubscription,
  getSubscriptions,
  updateSubscription,
} from './api'
import type { CreateSubscriptionRequest, UpdateSubscriptionRequest } from './types'

const subscriptionsQueryKey = ['subscriptions']

export function useSubscriptions() {
  return useQuery({ queryKey: subscriptionsQueryKey, queryFn: getSubscriptions })
}

export function useCreateSubscription() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateSubscriptionRequest) => createSubscription(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: subscriptionsQueryKey })
      toast.success(es.subscriptions.toasts.created)
    },
  })
}

export function useUpdateSubscription() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateSubscriptionRequest }) =>
      updateSubscription(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: subscriptionsQueryKey })
      toast.success(es.subscriptions.toasts.updated)
    },
  })
}

export function useDeleteSubscription() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteSubscription(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: subscriptionsQueryKey })
      toast.success(es.subscriptions.toasts.deleted)
    },
  })
}
