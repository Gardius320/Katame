/**
 * Lista de bancos y billeteras digitales más usados en Colombia, para el
 * buscador de "Banco" en el formulario de tarjetas. Los colores son
 * aproximaciones para armar una insignia visual distintiva -- no son los
 * logos oficiales (esos los sube el usuario si quiere, vía "Subir logo").
 */
export interface BankPreset {
  name: string
  color: string
}

export const BANK_PRESETS: BankPreset[] = [
  { name: 'Bancolombia', color: '#FFD200' },
  { name: 'Davivienda', color: '#EE3524' },
  { name: 'Banco de Bogotá', color: '#FFCC00' },
  { name: 'BBVA Colombia', color: '#004481' },
  { name: 'Banco de Occidente', color: '#004B87' },
  { name: 'Banco Popular', color: '#009045' },
  { name: 'Banco Caja Social', color: '#F58220' },
  { name: 'Scotiabank Colpatria', color: '#EC111A' },
  { name: 'Banco AV Villas', color: '#E4032E' },
  { name: 'Banco Agrario', color: '#2E7D32' },
  { name: 'Itaú', color: '#EC7000' },
  { name: 'GNB Sudameris', color: '#003DA5' },
  { name: 'Bancoomeva', color: '#7A1FA2' },
  { name: 'Banco Falabella', color: '#00934F' },
  { name: 'Banco Pichincha', color: '#FFC20E' },
  { name: 'Banco Mundo Mujer', color: '#EF7D00' },
  { name: 'Nu', color: '#820AD1' },
  { name: 'Nequi', color: '#FF0066' },
  { name: 'Daviplata', color: '#EE3524' },
  { name: 'Movii', color: '#00C389' },
  { name: 'Lulo Bank', color: '#00E0B8' },
  { name: 'RappiPay', color: '#FF441F' },
]

const SKIPPED_WORDS = new Set(['de', 'del', 'la', 'los', 'las'])

/** Iniciales para la insignia de color -- no depende de ningún logo real. */
export function getBankInitials(name: string): string {
  const words = name.split(/\s+/).filter((word) => !SKIPPED_WORDS.has(word.toLowerCase()))

  if (words.length >= 2) {
    return (words[0][0] + words[1][0]).toUpperCase()
  }

  return (words[0] ?? '').slice(0, 2).toUpperCase()
}

export function findBankPreset(name: string | null | undefined): BankPreset | undefined {
  if (!name) return undefined
  const normalized = name.trim().toLowerCase()
  return BANK_PRESETS.find((bank) => bank.name.toLowerCase() === normalized)
}
