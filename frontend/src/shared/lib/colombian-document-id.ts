/**
 * Valida una cédula colombiana: solo dígitos, entre 6 y 10 caracteres,
 * sin cero a la izquierda. A diferencia de la cédula ecuatoriana,
 * Colombia no usa un algoritmo de dígito verificador ni código de
 * provincia: la cédula de ciudadanía es un número secuencial asignado
 * por la Registraduría Nacional, así que la validación se limita a
 * formato y longitud. Debe reflejar el mismo criterio que el backend
 * (Validators/ColombianDocumentId.cs).
 */
export function isValidColombianCedula(value: string): boolean {
  if (!/^\d{6,10}$/.test(value)) return false
  if (value[0] === '0') return false
  return true
}

export const colombianPhoneRegex = /^3\d{9}$/
