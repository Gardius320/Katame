import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createObligation, deleteObligation, getObligations, updateObligation } from './api'
import type { CreateObligationRequest, UpdateObligationRequest } from './types'

const obligationsQueryKey = ['obligations']

export function useObligations() {
  return useQuery({ queryKey: obligationsQueryKey, queryFn: getObligations })
}

export function useCreateObligation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: CreateObligationRequest) => createObligation(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: obligationsQueryKey })
      toast.success(es.finance.obligations.toasts.created)
    },
  })
}

export function useUpdateObligation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateObligationRequest }) =>
      updateObligation(id, payload),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: obligationsQueryKey })
      toast.success(
        variables.payload.isPaid
          ? es.finance.obligations.toasts.paid
          : es.finance.obligations.toasts.updated,
      )
    },
  })
}

export function useDeleteObligation() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteObligation(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: obligationsQueryKey })
      toast.success(es.finance.obligations.toasts.deleted)
    },
  })
}
