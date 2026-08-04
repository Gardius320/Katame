import { es } from '@/shared/i18n/es'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/shared/components/ui/tabs'
import TransactionsPage from './transactions/transactions-page'
import SavingsPage from './savings/savings-page'
import ObligationsPage from './obligations/obligations-page'
import CreditCardsPage from './credit-cards/credit-cards-page'

export default function FinancePage() {
  return (
    <Tabs defaultValue="transactions" className="gap-6">
      <TabsList className="w-full sm:w-fit">
        <TabsTrigger value="transactions">{es.finance.tabs.transactions}</TabsTrigger>
        <TabsTrigger value="savings">{es.finance.tabs.savings}</TabsTrigger>
        <TabsTrigger value="obligations">{es.finance.tabs.obligations}</TabsTrigger>
        <TabsTrigger value="creditCards">{es.finance.tabs.creditCards}</TabsTrigger>
      </TabsList>

      <TabsContent value="transactions">
        <TransactionsPage />
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
  )
}
