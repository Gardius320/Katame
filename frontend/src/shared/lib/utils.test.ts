import { describe, expect, it } from 'vitest'
import { cn } from './utils'

describe('cn', () => {
  it('combina clases simples', () => {
    expect(cn('a', 'b')).toBe('a b')
  })

  it('resuelve conflictos de Tailwind quedándose con la última clase', () => {
    expect(cn('bg-red-500', 'bg-primary')).toBe('bg-primary')
  })

  it('ignora valores falsy', () => {
    const condition = 1 > 2
    expect(cn('a', condition && 'b', undefined, 'c')).toBe('a c')
  })
})
