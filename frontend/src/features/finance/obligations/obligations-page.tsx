import { useState } from 'react'
import { format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Check, Pencil, Plus, Repeat, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import { useDeleteObligation, useObligations, useUpdateObligation } from './hooks'
import { ObligationFormDialog } from './obligation-form-dialog'
import type { Obligation } from './types'
import { Button } from '@/shared/components/ui/button'
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

function formatDueDate(date: string): string {
  return format(new Date(date), "d 'de' MMMM", { locale: dateFnsEs })
}

export default function ObligationsPage() {
  const { data: obligations, isLoading } = useObligations()
  const updateObligation = useUpdateObligation()
  const deleteObligation = useDeleteObligation()

  const [formOpen, setFormOpen] = useState(false)
  const [editingObligation, setEditingObligation] = useState<Obligation | null>(null)
  const [obligationToDelete, setObligationToDelete] = useState<Obligation | null>(null)

  const openCreateForm = () => {
    setEditingObligation(null)
    setFormOpen(true)
  }

  const openEditForm = (obligation: Obligation) => {
    setEditingObligation(obligation)
    setFormOpen(true)
  }

  const togglePaid = (obligation: Obligation) => {
    updateObligation.mutate({
      id: obligation.id,
      payload: {
        name: obligation.name,
        amount: obligation.amount,
        dueDate: obligation.dueDate,
        isRecurring: obligation.isRecurring,
        isPaid: !obligation.isPaid,
      },
    })
  }

  const confirmDelete = () => {
    if (!obligationToDelete) return
    deleteObligation.mutate(obligationToDelete.id, { onSuccess: () => setObligationToDelete(null) })
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.finance.obligations.title}</h1>
          <p className="text-muted-foreground">{es.finance.obligations.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.finance.obligations.newObligation}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-2">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-16 w-full rounded-xl" />
          ))}
        </div>
      ) : obligations && obligations.length > 0 ? (
        <div className="grid gap-2">
          {obligations.map((obligation) => (
            <Card
              key={obligation.id}
              className="flex-row items-center justify-between gap-4 px-4 py-4"
            >
              <div className="flex min-w-0 items-center gap-3">
                <Badge
                  className={cn(
                    'gap-1',
                    obligation.isPaid
                      ? 'bg-positive/15 text-positive katame-seal'
                      : 'bg-destructive/15 text-destructive',
                  )}
                >
                  {obligation.isPaid && <Check className="size-3" />}
                  {obligation.isPaid ? es.finance.obligations.paid : es.finance.obligations.unpaid}
                </Badge>
                {obligation.isRecurring && (
                  <Badge variant="outline" className="gap-1">
                    <Repeat className="size-3" />
                    {es.finance.obligations.recurring}
                  </Badge>
                )}
                <div className="min-w-0">
                  <p className="truncate font-medium">{obligation.name}</p>
                  <p className="font-numeric text-xs text-muted-foreground">
                    {formatDueDate(obligation.dueDate)} · {formatCurrency(obligation.amount)}
                  </p>
                </div>
              </div>
              <div className="flex shrink-0 items-center gap-1">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => togglePaid(obligation)}
                  disabled={updateObligation.isPending}
                >
                  {obligation.isPaid
                    ? es.finance.obligations.markAsUnpaid
                    : es.finance.obligations.markAsPaid}
                </Button>
                <Button
                  variant="ghost"
                  size="icon-sm"
                  aria-label={es.common.edit}
                  title={es.common.edit}
                  onClick={() => openEditForm(obligation)}
                >
                  <Pencil className="size-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon-sm"
                  aria-label={es.common.delete}
                  title={es.common.delete}
                  onClick={() => setObligationToDelete(obligation)}
                >
                  <Trash2 className="size-4" />
                </Button>
              </div>
            </Card>
          ))}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">
          {es.finance.obligations.emptyState}
        </Card>
      )}

      <ObligationFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        obligation={editingObligation}
      />

      <AlertDialog
        open={obligationToDelete !== null}
        onOpenChange={(open) => !open && setObligationToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteObligation.isPending}>
              {deleteObligation.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
