export interface CreditCard {
  id: number
  name: string
  /** Nombre del banco emisor (para el buscador con insignia de color). */
  bank: string | null
  statementDay: number
  paymentDay: number
  creditLimit: number
  /** Gastado con esta tarjeta desde el último corte hasta hoy (ciclo abierto). */
  cycleUsage: number
  /** Logo del banco como data URL (base64), o null si no se subió ninguno. */
  logoDataUrl: string | null
}

export interface CreditCardFormValues {
  name: string
  bank: string | null
  statementDay: number
  paymentDay: number
  creditLimit: number
  logoDataUrl: string | null
}
