import { useState } from 'react'
import { CreditCard as CreditCardIcon, Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { useCreditCards, useDeleteCreditCard } from './hooks'
import { CreditCardFormDialog } from './credit-card-form-dialog'
import { findBankPreset, getBankInitials } from './bank-presets'
import type { CreditCard } from './types'
import { Button } from '@/shared/components/ui/button'
import { Card } from '@/shared/components/ui/card'
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

// Insignia del banco: prioriza el logo real subido por el usuario, luego la
// insignia de color con iniciales (si el banco coincide con la lista de
// bancos conocidos) y por último un ícono genérico.
function CardBankBadge({ card }: { card: CreditCard }) {
  if (card.logoDataUrl) {
    return (
      <img src={card.logoDataUrl} alt="" className="size-5 shrink-0 rounded object-contain" />
    )
  }

  const preset = findBankPreset(card.bank)
  if (preset) {
    return (
      <span
        className="flex size-5 shrink-0 items-center justify-center rounded-full text-[9px] font-semibold text-white"
        style={{ backgroundColor: preset.color }}
      >
        {getBankInitials(preset.name)}
      </span>
    )
  }

  return <CreditCardIcon className="size-5 shrink-0 text-primary" />
}

export default function CreditCardsPage() {
  const { data: cards, isLoading } = useCreditCards()
  const deleteCard = useDeleteCreditCard()

  const [formOpen, setFormOpen] = useState(false)
  const [editingCard, setEditingCard] = useState<CreditCard | null>(null)
  const [cardToDelete, setCardToDelete] = useState<CreditCard | null>(null)

  const openCreateForm = () => {
    setEditingCard(null)
    setFormOpen(true)
  }

  const openEditForm = (card: CreditCard) => {
    setEditingCard(card)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!cardToDelete) return
    deleteCard.mutate(cardToDelete.id, { onSuccess: () => setCardToDelete(null) })
  }

  return (
    <div className="grid gap-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="font-heading text-2xl font-semibold">{es.finance.creditCards.title}</h1>
          <p className="text-muted-foreground">{es.finance.creditCards.subtitle}</p>
        </div>
        <Button onClick={openCreateForm}>
          <Plus className="size-4" />
          {es.finance.creditCards.newCard}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {Array.from({ length: 2 }).map((_, index) => (
            <Skeleton key={index} className="h-32 w-full rounded-xl" />
          ))}
        </div>
      ) : cards && cards.length > 0 ? (
        <div className="grid gap-3 sm:grid-cols-2">
          {cards.map((card) => (
            <Card key={card.id} className="min-w-0 gap-3 p-4">
              <div className="flex items-start justify-between gap-2">
                <div className="flex items-center gap-2">
                  <CardBankBadge card={card} />
                  <div>
                    <p className="font-heading font-semibold">{card.name}</p>
                    {card.bank && (
                      <p className="text-xs text-muted-foreground">{card.bank}</p>
                    )}
                  </div>
                </div>
                <div className="flex shrink-0 items-center gap-1">
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.edit}
                    title={es.common.edit}
                    onClick={() => openEditForm(card)}
                  >
                    <Pencil className="size-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    aria-label={es.common.delete}
                    title={es.common.delete}
                    onClick={() => setCardToDelete(card)}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-2 font-numeric text-sm text-muted-foreground">
                <p>
                  {es.finance.creditCards.statementDayLabel.replace(
                    '{day}',
                    String(card.statementDay),
                  )}
                </p>
                <p>
                  {es.finance.creditCards.paymentDayLabel.replace('{day}', String(card.paymentDay))}
                </p>
              </div>

              <div className="grid grid-cols-2 gap-2 font-numeric text-sm">
                <p
                  className={
                    card.cycleUsage > card.creditLimit
                      ? 'font-semibold text-destructive'
                      : undefined
                  }
                >
                  {es.finance.creditCards.cycleUsageLabel}: {formatCurrency(card.cycleUsage)}
                </p>
                <p>
                  {es.finance.creditCards.limitLabel}: {formatCurrency(card.creditLimit)}
                </p>
              </div>

              {card.cycleUsage > card.creditLimit && (
                <p className="text-xs font-medium text-destructive">
                  {es.finance.creditCards.overLimitLabel}
                </p>
              )}
            </Card>
          ))}
        </div>
      ) : (
        <Card className="p-10 text-center text-muted-foreground">
          {es.finance.creditCards.emptyState}
        </Card>
      )}

      <CreditCardFormDialog open={formOpen} onOpenChange={setFormOpen} card={editingCard} />

      <AlertDialog
        open={cardToDelete !== null}
        onOpenChange={(open) => !open && setCardToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteCard.isPending}>
              {deleteCard.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
