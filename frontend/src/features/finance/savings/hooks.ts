import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createSavingsGoal, deleteSavingsGoal, getSavingsGoals, updateSavingsGoal } from './api'
import type { SavingsGoalFormValues } from './types'

const savingsQueryKey = ['savings-goals']

export function useSavingsGoals() {
  return useQuery({ queryKey: savingsQueryKey, queryFn: getSavingsGoals })
}

export function useCreateSavingsGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: SavingsGoalFormValues) => createSavingsGoal(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: savingsQueryKey })
      toast.success(es.finance.savings.toasts.created)
    },
  })
}

export function useUpdateSavingsGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: SavingsGoalFormValues }) =>
      updateSavingsGoal(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: savingsQueryKey })
      toast.success(es.finance.savings.toasts.updated)
    },
  })
}

export function useDeleteSavingsGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteSavingsGoal(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: savingsQueryKey })
      toast.success(es.finance.savings.toasts.deleted)
    },
  })
}
