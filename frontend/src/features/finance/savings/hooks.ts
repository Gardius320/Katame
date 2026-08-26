import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import {
  contributeSavingsGoal,
  createSavingsGoal,
  deleteSavingsGoal,
  getFinancialProfile,
  getSavingsGoals,
  updateFinancialProfile,
  updateSavingsGoal,
} from './api'
import type { ContributeSavingsGoalValues, FinancialProfile, SavingsGoalFormValues } from './types'

const savingsQueryKey = ['savings-goals']
const financialProfileQueryKey = ['financial-profile']

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

export function useContributeSavingsGoal() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: ContributeSavingsGoalValues }) =>
      contributeSavingsGoal(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: savingsQueryKey })
      toast.success(es.finance.savings.toasts.contributed)
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

export function useFinancialProfile() {
  return useQuery({ queryKey: financialProfileQueryKey, queryFn: getFinancialProfile })
}

export function useUpdateFinancialProfile() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: FinancialProfile) => updateFinancialProfile(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: financialProfileQueryKey })
      toast.success(es.finance.savings.toasts.profileUpdated)
    },
  })
}
