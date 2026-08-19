import { describe, expect, it } from 'vitest'
import { colombianPhoneRegex, isValidColombianCedula } from './colombian-document-id'

describe('isValidColombianCedula', () => {
  it.each(['1001234567', '123456', '999999999', '1234567890'])(
    'acepta cédulas con formato válido (%s)',
    (cedula) => {
      expect(isValidColombianCedula(cedula)).toBe(true)
    },
  )

  it.each([
    ['', 'vacía'],
    ['12345', 'longitud incorrecta (5 dígitos)'],
    ['12345678901', 'longitud incorrecta (11 dígitos)'],
    ['170123456A', 'no numérica'],
    ['0123456789', 'no puede empezar en 0'],
  ])('rechaza %s (%s)', (cedula) => {
    expect(isValidColombianCedula(cedula)).toBe(false)
  })
})

describe('colombianPhoneRegex', () => {
  it.each(['3001234567', '3159876543'])('acepta teléfonos válidos (%s)', (phone) => {
    expect(colombianPhoneRegex.test(phone)).toBe(true)
  })

  it.each(['12345', '0991234567', '30012345678', ''])(
    'rechaza teléfonos inválidos (%s)',
    (phone) => {
      expect(colombianPhoneRegex.test(phone)).toBe(false)
    },
  )
})
