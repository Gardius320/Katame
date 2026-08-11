/**
 * Valida una cédula ecuatoriana: 10 dígitos, código de provincia (01-24),
 * tercer dígito de persona natural (0-6) y dígito verificador (módulo 10).
 * Debe reflejar el mismo algoritmo que el backend (Validators/EcuadorianDocumentId.cs).
 */
export function isValidEcuadorianCedula(value: string): boolean {
  if (!/^\d{10}$/.test(value)) return false

  const digits = value.split('').map(Number)

  const province = digits[0] * 10 + digits[1]
  if (province < 1 || province > 24) return false

  if (digits[2] > 6) return false

  const coefficients = [2, 1, 2, 1, 2, 1, 2, 1, 2]
  let sum = 0
  for (let i = 0; i < 9; i++) {
    let product = digits[i] * coefficients[i]
    if (product > 9) product -= 9
    sum += product
  }

  const verifier = (10 - (sum % 10)) % 10
  return verifier === digits[9]
}

export const ecuadorianPhoneRegex = /^(09\d{8}|0[2-7]\d{7})$/
