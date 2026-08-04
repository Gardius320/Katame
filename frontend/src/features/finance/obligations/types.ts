export interface Obligation {
  id: number
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
  isPaid: boolean
}

export interface CreateObligationRequest {
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
}

export interface UpdateObligationRequest {
  name: string
  amount: number
  dueDate: string
  isRecurring: boolean
  isPaid: boolean
}
