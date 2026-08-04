import { useMemo, useRef, useState } from 'react'
import { useVirtualizer } from '@tanstack/react-virtual'
import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Download, Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import {
  useDeleteTransaction,
  useExportTransactionsCsv,
  useTransactions,
  useTransactionsSummary,
} from './hooks'
import { TransactionFormDialog } from './transaction-form-dialog'
import { TransactionsChart } from './transactions-chart'
import type { Transaction, TransactionFilter } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Card } from '@/shared/components/ui/card'
import { Badge } from '@/shared/components/ui/badge'
import { Skeleton } from '@/shared/components/ui/skeleton'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/shared/components/ui/alert-dialog'

const PAGE_SIZE = 20
const ROW_HEIGHT = 60

function formatTransactionDate(date: string): string {
  return format(new Date(date), 'd MMM yyyy', { locale: dateFnsEs })
}

export default function TransactionsPage() {
  const [filter, setFilter] = useState<TransactionFilter>({})
  const [page, setPage] = useState(1)

  const { data, isLoading } = useTransactions(filter, page, PAGE_SIZE)
  const { data: summaryData } = useTransactionsSummary(filter)
  const deleteTransaction = useDeleteTransaction()
  const exportCsv = useExportTransactionsCsv()

  const [formOpen, setFormOpen] = useState(false)
  const [editingTransaction, setEditingTransaction] = useState<Transaction | null>(null)
  const [transactionToDelete, setTransactionToDelete] = useState<Transaction | null>(null)

  const items = useMemo(() => data?.items ?? [], [data])
  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / PAGE_SIZE)) : 1

  const scrollRef = useRef<HTMLDivElement>(null)
  const virtualizer = useVirtualizer({
    count: items.length,
    getScrollElement: () => scrollRef.current,
    estimateSize: () => ROW_HEIGHT,
  })

  const openCreateForm = () => {
    setEditingTransaction(null)
    setFormOpen(true)
  }

  const openEditForm = (transaction: Transaction) => {
    setEditingTransaction(transaction)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!transactionToDelete) return
    deleteTransaction.mutate(transactionToDelete.id, {
      onSuccess: () => setTransactionToDelete(null),
    })
  }

  const updateFilter = (patch: Partial<TransactionFilter>) => {
    setPage(1)
    setFilter((prev) => ({ ...prev, ...patch }))
  }

  const clearFilters = () => {
    setPage(1)
    setFilter({})
  }

  return (
    <div className="grid gap-6">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.finance.title}</h1>
          <p className="text-muted-foreground">{es.finance.subtitle}</p>
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            onClick={() => exportCsv.mutate(filter)}
            disabled={exportCsv.isPending}
          >
            <Download className="size-4" />
            {es.finance.transactions.exportCsv}
          </Button>
          <Button onClick={openCreateForm}>
            <Plus className="size-4" />
            {es.finance.transactions.newTransaction}
          </Button>
        </div>
      </div>

      <TransactionsChart transactions={summaryData?.items ?? []} />

      <Card className="grid grid-cols-2 gap-3 p-4 sm:grid-cols-4">
        <div className="grid gap-1.5">
          <label className="text-sm font-medium text-muted-foreground">
            {es.finance.transactions.filters.startDate}
          </label>
          <Input
            type="date"
            className="font-numeric"
            value={filter.startDate ?? ''}
            onChange={(e) => updateFilter({ startDate: e.target.value || undefined })}
          />
        </div>
        <div className="grid gap-1.5">
          <label className="text-sm font-medium text-muted-foreground">
            {es.finance.transactions.filters.endDate}
          </label>
          <Input
            type="date"
            className="font-numeric"
            value={filter.endDate ?? ''}
            onChange={(e) => updateFilter({ endDate: e.target.value || undefined })}
          />
        </div>
        <div className="grid gap-1.5">
          <label className="text-sm font-medium text-muted-foreground">
            {es.finance.transactions.filters.category}
          </label>
          <Input
            placeholder={es.finance.transactions.filters.categoryPlaceholder}
            value={filter.category ?? ''}
            onChange={(e) => updateFilter({ category: e.target.value || undefined })}
          />
        </div>
        <div className="flex items-end">
          <Button variant="ghost" onClick={clearFilters} className="w-full">
            {es.finance.transactions.filters.clear}
          </Button>
        </div>
      </Card>

      {isLoading ? (
        <div className="grid gap-2">
          {Array.from({ length: 5 }).map((_, index) => (
            <Skeleton key={index} className="h-14 w-full rounded-xl" />
          ))}
        </div>
      ) : items.length > 0 ? (
        <>
          <div
            ref={scrollRef}
            className="max-h-[480px] overflow-y-auto rounded-xl border border-border"
          >
            <div style={{ height: virtualizer.getTotalSize(), position: 'relative' }}>
              {virtualizer.getVirtualItems().map((virtualRow) => {
                const transaction = items[virtualRow.index]
                return (
                  <div
                    key={transaction.id}
                    style={{
                      position: 'absolute',
                      top: 0,
                      left: 0,
                      width: '100%',
                      height: virtualRow.size,
                      transform: `translateY(${virtualRow.start}px)`,
                    }}
                    className="flex items-center justify-between gap-3 border-b border-border bg-card px-4 last:border-b-0"
                  >
                    <div className="flex min-w-0 items-center gap-3">
                      <Badge
                        className={cn(
                          transaction.type === 'income'
                            ? 'bg-positive/15 text-positive'
                            : 'bg-destructive/15 text-destructive',
                        )}
                      >
                        {es.finance.transactions.type[transaction.type]}
                      </Badge>
                      <div className="min-w-0">
                        <p className="truncate font-medium">{transaction.category}</p>
                        <p className="font-numeric text-xs text-muted-foreground">
                          {formatTransactionDate(transaction.date)}
                        </p>
                      </div>
                    </div>
                    <div className="flex shrink-0 items-center gap-2">
                      <p
                        className={cn(
                          'font-numeric font-medium',
                          transaction.type === 'income' ? 'text-positive' : 'text-destructive',
                        )}
                      >
                        {transaction.type === 'income' ? '+' : '-'}
                        {formatCurrency(transaction.amount)}
                      </p>
                      <Button
                        variant="ghost"
                        size="icon-sm"
                        aria-label={es.common.edit}
                        title={es.common.edit}
                        onClick={() => openEditForm(transaction)}
                      >
                        <Pencil className="size-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon-sm"
                        aria-label={es.common.delete}
                        title={es.common.delete}
                        onClick={() => setTransactionToDelete(transaction)}
                      >
                        <Trash2 className="size-4" />
                      </Button>
                    </div>
                  </div>
                )
              })}
            </div>
          </div>

          <div className="flex items-center justify-between">
            <p className="text-sm text-muted-foreground">
              {es.finance.transactions.pagination.pageInfo
                .replace('{page}', String(page))
                .replace('{totalPages}', String(totalPages))}
            </p>
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                disabled={page <= 1}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
              >
                {es.finance.transactions.pagination.previous}
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              >
                {es.finance.transactions.pagination.next}
              </Button>
            </div>
          </div>
        </>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">
          {es.finance.transactions.emptyState}
        </Card>
      )}

      <TransactionFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        transaction={editingTransaction}
      />

      <AlertDialog
        open={transactionToDelete !== null}
        onOpenChange={(open) => !open && setTransactionToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteTransaction.isPending}>
              {deleteTransaction.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
