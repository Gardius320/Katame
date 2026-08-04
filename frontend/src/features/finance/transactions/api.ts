import { apiClient } from '@/shared/api/client'
import type { PagedResult, Transaction, TransactionFilter, TransactionFormValues } from './types'

export async function getTransactions(
  filter: TransactionFilter,
  page: number,
  pageSize: number,
): Promise<PagedResult<Transaction>> {
  const { data } = await apiClient.get<PagedResult<Transaction>>('/finance/transactions', {
    params: { ...filter, page, pageSize },
  })
  return data
}

export async function createTransaction(payload: TransactionFormValues): Promise<Transaction> {
  const { data } = await apiClient.post<Transaction>('/finance/transactions', payload)
  return data
}

export async function updateTransaction(
  id: number,
  payload: TransactionFormValues,
): Promise<Transaction> {
  const { data } = await apiClient.put<Transaction>(`/finance/transactions/${id}`, payload)
  return data
}

export async function deleteTransaction(id: number): Promise<void> {
  await apiClient.delete(`/finance/transactions/${id}`)
}

export async function exportTransactionsCsv(filter: TransactionFilter): Promise<Blob> {
  const { data } = await apiClient.get('/finance/transactions/export', {
    params: filter,
    responseType: 'blob',
  })
  return data
}
