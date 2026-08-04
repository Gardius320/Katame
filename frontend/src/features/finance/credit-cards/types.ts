export interface CreditCard {
  id: number
  name: string
  statementDay: number
  paymentDay: number
  creditLimit: number
}

export interface CreditCardFormValues {
  name: string
  statementDay: number
  paymentDay: number
  creditLimit: number
}
