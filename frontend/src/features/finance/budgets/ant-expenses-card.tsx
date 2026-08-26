import { Bug } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { Card } from '@/shared/components/ui/card'
import { useAntExpenses } from './hooks'

/**
 * Aviso de "gastos hormiga": categorías con transacciones chicas y
 * frecuentes en lo que va del mes, que entre todas suman más de lo que
 * parece. No reemplaza a los presupuestos -- aplica incluso a categorías sin
 * uno asignado, porque el problema es el patrón de gasto, no el límite.
 */
export function AntExpensesCard() {
  const { data: antExpenses } = useAntExpenses()

  if (!antExpenses || antExpenses.length === 0) {
    return null
  }

  return (
    <Card className="gap-3 border-amber-500/30 bg-amber-500/5 p-4">
      <div className="flex items-start gap-3">
        <Bug className="mt-0.5 size-5 shrink-0 text-amber-500" />
        <div className="min-w-0">
          <p className="font-heading font-semibold text-amber-600 dark:text-amber-400">
            {es.finance.budgets.antExpenses.title}
          </p>
          <p className="text-sm text-muted-foreground">{es.finance.budgets.antExpenses.description}</p>
        </div>
      </div>

      <ul className="grid gap-2">
        {antExpenses.map((expense) => (
          <li
            key={expense.category}
            className="flex items-center justify-between gap-2 rounded-lg bg-background/60 px-3 py-2"
          >
            <div className="min-w-0">
              <p className="truncate text-sm font-medium">{expense.category}</p>
              <p className="font-numeric text-xs text-muted-foreground">
                {es.finance.budgets.antExpenses.frequencyLabel.replace(
                  '{count}',
                  String(expense.transactionCount),
                )}
              </p>
            </div>
            <p className="font-numeric shrink-0 text-sm font-semibold">
              {formatCurrency(expense.totalAmount)}
            </p>
          </li>
        ))}
      </ul>
    </Card>
  )
}
