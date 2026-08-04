import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import type { Transaction } from './types'

interface TransactionsChartProps {
  transactions: Transaction[]
}

export function TransactionsChart({ transactions }: TransactionsChartProps) {
  const income = transactions
    .filter((t) => t.type === 'income')
    .reduce((sum, t) => sum + t.amount, 0)
  const expense = transactions
    .filter((t) => t.type === 'expense')
    .reduce((sum, t) => sum + t.amount, 0)

  const data = [
    { name: es.finance.transactions.summary.income, value: income, fill: 'var(--positive)' },
    { name: es.finance.transactions.summary.expense, value: expense, fill: 'var(--destructive)' },
  ]

  return (
    <div className="rounded-xl border border-border bg-card p-4">
      <div className="mb-3 flex items-center justify-between">
        <h2 className="font-heading text-sm font-semibold text-muted-foreground">
          {es.finance.transactions.chartTitle}
        </h2>
        <p className="font-numeric text-sm font-medium">
          {es.finance.transactions.summary.balance}: {formatCurrency(income - expense)}
        </p>
      </div>
      <ResponsiveContainer width="100%" height={160}>
        <BarChart data={data} layout="vertical" margin={{ left: 8, right: 16 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" horizontal={false} />
          <XAxis type="number" tick={{ fill: 'var(--muted-foreground)', fontSize: 12 }} />
          <YAxis
            type="category"
            dataKey="name"
            width={70}
            tick={{ fill: 'var(--muted-foreground)', fontSize: 12 }}
          />
          <Tooltip
            formatter={(value) => formatCurrency(Number(value))}
            contentStyle={{
              backgroundColor: 'var(--popover)',
              border: '1px solid var(--border)',
              borderRadius: 8,
              color: 'var(--popover-foreground)',
            }}
          />
          <Bar dataKey="value" radius={[0, 6, 6, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  )
}
