export type TransactionType = 'income' | 'expense'

export interface Transaction {
  id: number
  amount: number
  type: TransactionType
  category: string
  date: string
}

export interface TransactionFilter {
  startDate?: string
  endDate?: string
  category?: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
}

export interface TransactionFormValues {
  amount: number
  type: TransactionType
  category: string
  date: string
}
