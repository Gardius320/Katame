import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createCreditCard, deleteCreditCard, getCreditCards, updateCreditCard } from './api'
import type { CreditCardFormValues } from './types'

const creditCardsQueryKey = ['credit-cards']

export function useCreditCards() {
  return useQuery({ queryKey: creditCardsQueryKey, queryFn: getCreditCards })
}

export function useCreateCreditCard() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreditCardFormValues) => createCreditCard(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: creditCardsQueryKey })
      toast.success(es.finance.creditCards.toasts.created)
    },
  })
}

export function useUpdateCreditCard() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: CreditCardFormValues }) =>
      updateCreditCard(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: creditCardsQueryKey })
      toast.success(es.finance.creditCards.toasts.updated)
    },
  })
}

export function useDeleteCreditCard() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteCreditCard(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: creditCardsQueryKey })
      toast.success(es.finance.creditCards.toasts.deleted)
    },
  })
}
