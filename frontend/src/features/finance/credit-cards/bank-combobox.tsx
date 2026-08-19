import { useEffect, useRef, useState } from 'react'
import { Check, ChevronDown, X } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { Input } from '@/shared/components/ui/input'
import { BANK_PRESETS, getBankInitials } from './bank-presets'
import { cn } from '@/shared/lib/utils'

interface BankComboboxProps {
  value: string | null
  onChange: (value: string | null) => void
}

/**
 * Buscador de bancos hecho a mano: el proyecto no tiene un primitivo tipo
 * Popover/Command, así que esto es un input + lista filtrada posicionada de
 * forma absoluta, con cierre al hacer click afuera. Cada banco se muestra
 * con una insignia de color + iniciales (no logos reales, ver bank-presets.ts).
 */
export function BankCombobox({ value, onChange }: BankComboboxProps) {
  const [query, setQuery] = useState(value ?? '')
  const [isOpen, setIsOpen] = useState(false)
  const containerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    setQuery(value ?? '')
  }, [value])

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false)
        setQuery(value ?? '')
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [value])

  const normalizedQuery = query.trim().toLowerCase()
  const filteredBanks = normalizedQuery
    ? BANK_PRESETS.filter((bank) => bank.name.toLowerCase().includes(normalizedQuery))
    : BANK_PRESETS

  const handleSelect = (bankName: string) => {
    onChange(bankName)
    setQuery(bankName)
    setIsOpen(false)
  }

  const handleClear = () => {
    onChange(null)
    setQuery('')
    setIsOpen(false)
  }

  return (
    <div ref={containerRef} className="relative">
      <div className="relative">
        <Input
          value={query}
          placeholder={es.finance.creditCards.fields.bankPlaceholder}
          onFocus={() => setIsOpen(true)}
          onChange={(e) => {
            setQuery(e.target.value)
            setIsOpen(true)
            if (e.target.value.trim() === '') {
              onChange(null)
            }
          }}
          className="pr-16"
        />
        <div className="absolute inset-y-0 right-2 flex items-center gap-1">
          {value && (
            <button
              type="button"
              onClick={handleClear}
              className="rounded-sm p-0.5 text-muted-foreground hover:text-foreground"
              aria-label={es.finance.creditCards.fields.bankClear}
            >
              <X className="size-3.5" />
            </button>
          )}
          <ChevronDown className="size-3.5 text-muted-foreground" />
        </div>
      </div>

      {isOpen && (
        <div className="absolute z-50 mt-1 max-h-64 w-full overflow-auto rounded-md border border-border bg-popover p-1 shadow-md">
          {filteredBanks.length === 0 ? (
            <p className="px-2 py-3 text-center text-sm text-muted-foreground">
              {es.finance.creditCards.fields.bankNoResults}
            </p>
          ) : (
            filteredBanks.map((bank) => (
              <button
                key={bank.name}
                type="button"
                onClick={() => handleSelect(bank.name)}
                className={cn(
                  'flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-left text-sm hover:bg-accent',
                  value === bank.name && 'bg-accent',
                )}
              >
                <span
                  className="flex size-6 shrink-0 items-center justify-center rounded-full text-[10px] font-semibold text-white"
                  style={{ backgroundColor: bank.color }}
                >
                  {getBankInitials(bank.name)}
                </span>
                <span className="flex-1 truncate">{bank.name}</span>
                {value === bank.name && <Check className="size-4 shrink-0 text-primary" />}
              </button>
            ))
          )}
        </div>
      )}
    </div>
  )
}
