import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import { createBudget, deleteBudget, getAntExpenses, getBudgets, updateBudget } from './api'
import type { BudgetFormValues } from './types'

const budgetsQueryKey = ['budgets']
const antExpensesQueryKey = ['ant-expenses']

export function useBudgets() {
  return useQuery({ queryKey: budgetsQueryKey, queryFn: getBudgets })
}

export function useAntExpenses() {
  return useQuery({ queryKey: antExpensesQueryKey, queryFn: getAntExpenses })
}

export function useCreateBudget() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: BudgetFormValues) => createBudget(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: budgetsQueryKey })
      toast.success(es.finance.budgets.toasts.created)
    },
  })
}

export function useUpdateBudget() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: BudgetFormValues }) =>
      updateBudget(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: budgetsQueryKey })
      toast.success(es.finance.budgets.toasts.updated)
    },
  })
}

export function useDeleteBudget() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteBudget(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: budgetsQueryKey })
      toast.success(es.finance.budgets.toasts.deleted)
    },
  })
}
