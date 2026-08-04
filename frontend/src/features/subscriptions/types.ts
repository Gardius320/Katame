export interface Subscription {
  id: number
  name: string
  amount: number
  renewalDate: string
  reminderEnabled: boolean
}

export interface CreateSubscriptionRequest {
  name: string
  amount: number
  renewalDate: string
  reminderEnabled: boolean
}

export interface UpdateSubscriptionRequest {
  name: string
  amount: number
  renewalDate: string
  reminderEnabled: boolean
}
