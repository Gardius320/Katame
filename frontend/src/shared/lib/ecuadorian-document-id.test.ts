import { describe, expect, it } from 'vitest'
import { ecuadorianPhoneRegex, isValidEcuadorianCedula } from './ecuadorian-document-id'

describe('isValidEcuadorianCedula', () => {
  it.each(['1701234567', '1712345675', '1723456784', '0918273640'])(
    'acepta cédulas con dígito verificador correcto (%s)',
    (cedula) => {
      expect(isValidEcuadorianCedula(cedula)).toBe(true)
    },
  )

  it.each([
    ['', 'vacía'],
    ['123456789', 'longitud incorrecta (9 dígitos)'],
    ['12345678901', 'longitud incorrecta (11 dígitos)'],
    ['170123456A', 'no numérica'],
    ['1234567890', 'dígito verificador incorrecto'],
    ['9901234567', 'provincia inválida'],
    ['1791234567', 'tercer dígito de persona natural inválido'],
  ])('rechaza %s (%s)', (cedula) => {
    expect(isValidEcuadorianCedula(cedula)).toBe(false)
  })
})

describe('ecuadorianPhoneRegex', () => {
  it.each(['0991234567', '022345678'])('acepta teléfonos válidos (%s)', (phone) => {
    expect(ecuadorianPhoneRegex.test(phone)).toBe(true)
  })

  it.each(['12345', '0512345678', '08123456789', ''])(
    'rechaza teléfonos inválidos (%s)',
    (phone) => {
      expect(ecuadorianPhoneRegex.test(phone)).toBe(false)
    },
  )
})
