import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createGoal, deleteGoal, getGoals, updateGoal } from './api'
import type { GoalFormValues } from './types'

const goalsQueryKey = ['goals']

export function useGoals() {
  return useQuery({ queryKey: goalsQueryKey, queryFn: getGoals })
}

export function useCreateGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: GoalFormValues) => createGoal(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: goalsQueryKey })
      toast.success(es.goals.toasts.created)
    },
  })
}

export function useUpdateGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: GoalFormValues }) =>
      updateGoal(id, payload),
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: goalsQueryKey })
      toast.success(
        variables.payload.progressPercentage >= 100
          ? es.goals.toasts.completed
          : es.goals.toasts.updated,
      )
    },
  })
}

export function useDeleteGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteGoal(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: goalsQueryKey })
      toast.success(es.goals.toasts.deleted)
    },
  })
}
