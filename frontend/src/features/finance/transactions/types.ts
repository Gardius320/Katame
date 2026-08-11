export type TransactionType = 'income' | 'expense'

export interface Transaction {
  id: number
  amount: number
  type: TransactionType
  category: string
  date: string
  creditCardId: number | null
}

export interface TransactionFilter {
  startDate?: string
  endDate?: string
  category?: string
  creditCardId?: number
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
  creditCardId: number | null
}
