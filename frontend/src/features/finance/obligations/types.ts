export type RecurrenceFrequency = 'Biweekly' | 'Monthly'

export interface Obligation {
  id: number
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
  recurrenceFrequency: RecurrenceFrequency | null
  isPaid: boolean
}

export interface CreateObligationRequest {
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
  recurrenceFrequency: RecurrenceFrequency | null
}

export interface UpdateObligationRequest {
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
  recurrenceFrequency: RecurrenceFrequency | null
  isPaid: boolean
}
