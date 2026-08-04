import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { es } from '@/shared/i18n/es'
import {
  createTransaction,
  deleteTransaction,
  exportTransactionsCsv,
  getTransactions,
  updateTransaction,
} from './api'
import type { TransactionFilter, TransactionFormValues } from './types'

const transactionsQueryKey = (filter: TransactionFilter, page: number, pageSize: number) => [
  'transactions',
  filter,
  page,
  pageSize,
]

export function useTransactions(filter: TransactionFilter, page: number, pageSize: number) {
  return useQuery({
    queryKey: transactionsQueryKey(filter, page, pageSize),
    queryFn: () => getTransactions(filter, page, pageSize),
  })
}

const SUMMARY_PAGE_SIZE = 1000

export function useTransactionsSummary(filter: TransactionFilter) {
  return useQuery({
    queryKey: ['transactions-summary', filter],
    queryFn: () => getTransactions(filter, 1, SUMMARY_PAGE_SIZE),
  })
}

export function useCreateTransaction() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (payload: TransactionFormValues) => createTransaction(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['transactions-summary'] })
      toast.success(es.finance.transactions.toasts.created)
    },
  })
}

export function useUpdateTransaction() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: TransactionFormValues }) =>
      updateTransaction(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['transactions-summary'] })
      toast.success(es.finance.transactions.toasts.updated)
    },
  })
}

export function useDeleteTransaction() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => deleteTransaction(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
      queryClient.invalidateQueries({ queryKey: ['transactions-summary'] })
      toast.success(es.finance.transactions.toasts.deleted)
    },
  })
}

export function useExportTransactionsCsv() {
  return useMutation({
    mutationFn: (filter: TransactionFilter) => exportTransactionsCsv(filter),
    onSuccess: (blob) => {
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = 'transactions.csv'
      link.click()
      URL.revokeObjectURL(url)
      toast.success(es.finance.transactions.toasts.exported)
    },
  })
}
