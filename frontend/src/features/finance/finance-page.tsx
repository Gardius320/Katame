import { CalendarClock, Wallet } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/shared/components/ui/tabs'
import { StatCard } from '@/shared/components/ui/stat-card'
import { Card } from '@/shared/components/ui/card'
import { CircularGauge } from '@/shared/components/ui/circular-gauge'
import TransactionsPage from './transactions/transactions-page'
import SavingsPage from './savings/savings-page'
import ObligationsPage from './obligations/obligations-page'
import CreditCardsPage from './credit-cards/credit-cards-page'
import BudgetsPage from './budgets/budgets-page'
import { useTransactionsSummary } from './transactions/hooks'
import { useObligations } from './obligations/hooks'
import { useCreditCards } from './credit-cards/hooks'

// Corte mensual: statementDay es un día del mes (1-31), no una fecha real, así que el
// ciclo vigente se deriva a partir de hoy. Si el corte de este mes cae en un mes más corto
// (ej. día 31 en febrero), se clampea al último día disponible.
function statementDateFor(statementDay: number, year: number, month: number): Date {
  const daysInMonth = new Date(year, month + 1, 0).getDate()
  return new Date(year, month, Math.min(statementDay, daysInMonth))
}

function getCurrentBillingCycle(statementDay: number, referenceDate: Date): { start: Date; end: Date } {
  const year = referenceDate.getFullYear()
  const month = referenceDate.getMonth()
  const thisMonthStatement = statementDateFor(statementDay, year, month)

  if (referenceDate >= thisMonthStatement) {
    return { start: thisMonthStatement, end: statementDateFor(statementDay, year, month + 1) }
  }

  return { start: statementDateFor(statementDay, year, month - 1), end: thisMonthStatement }
}

export default function FinancePage() {
  const { data: summaryData } = useTransactionsSummary({})
  const { data: obligations } = useObligations()
  const { data: creditCards } = useCreditCards()

  const balance = (summaryData?.items ?? []).reduce(
    (sum, transaction) =>
      sum + (transaction.type === 'income' ? transaction.amount : -transaction.amount),
    0,
  )
  const upcomingCount = (obligations ?? []).filter((obligation) => !obligation.isPaid).length

  const mainCard = creditCards?.[0]
  const cycle = mainCard ? getCurrentBillingCycle(mainCard.statementDay, new Date()) : null
  const { data: cardCycleSummary } = useTransactionsSummary(
    mainCard && cycle
      ? {
          creditCardId: mainCard.id,
          startDate: cycle.start.toISOString(),
          endDate: cycle.end.toISOString(),
        }
      : {},
  )
  const cycleSpend = (cardCycleSummary?.items ?? [])
    .filter((transaction) => transaction.type === 'expense')
    .reduce((sum, transaction) => sum + transaction.amount, 0)
  const cardUsagePercent = mainCard
    ? Math.min(100, Math.max(0, (cycleSpend / mainCard.creditLimit) * 100))
    : 0
  const cardUsageVariant =
    cardUsagePercent > 90 ? 'danger' : cardUsagePercent >= 70 ? 'warning' : 'default'

  return (
    <div className="grid gap-6">
      <div className="grid gap-4 sm:grid-cols-3">
        <StatCard
          label={es.finance.transactions.summary.balance}
          value={formatCurrency(balance)}
          icon={Wallet}
          variant="hero"
        />
        <StatCard
          label={es.today.upcoming.title}
          value={upcomingCount}
          icon={CalendarClock}
          variant={upcomingCount > 0 ? 'warning' : 'default'}
        />
        {mainCard && (
          <Card className="flex flex-col items-center justify-center gap-2 p-4">
            <CircularGauge
              value={cycleSpend}
              max={mainCard.creditLimit}
              label={mainCard.name}
              size="lg"
              variant={cardUsageVariant}
            />
            <p className="text-xs text-muted-foreground">{es.finance.creditCards.cycleUsageLabel}</p>
          </Card>
        )}
      </div>

      <Tabs defaultValue="transactions" className="gap-6">
        <TabsList className="w-full sm:w-fit">
          <TabsTrigger value="transactions">{es.finance.tabs.transactions}</TabsTrigger>
          <TabsTrigger value="budgets">{es.finance.tabs.budgets}</TabsTrigger>
          <TabsTrigger value="savings">{es.finance.tabs.savings}</TabsTrigger>
          <TabsTrigger value="obligations">{es.finance.tabs.obligations}</TabsTrigger>
          <TabsTrigger value="creditCards">{es.finance.tabs.creditCards}</TabsTrigger>
        </TabsList>

        <TabsContent value="transactions">
          <TransactionsPage />
        </TabsContent>
        <TabsContent value="budgets">
          <BudgetsPage />
        </TabsContent>
        <TabsContent value="savings">
          <SavingsPage />
        </TabsContent>
        <TabsContent value="obligations">
          <ObligationsPage />
        </TabsContent>
        <TabsContent value="creditCards">
          <CreditCardsPage />
        </TabsContent>
      </Tabs>
    </div>
  )
}
