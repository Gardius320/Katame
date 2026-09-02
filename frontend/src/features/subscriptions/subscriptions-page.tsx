import { useState } from 'react'
import { differenceInCalendarDays, format } from 'date-fns'
import { es as dateFnsEs } from 'date-fns/locale'
import { Bell, BellOff, Pencil, Plus, Trash2 } from 'lucide-react'
import { es } from '@/shared/i18n/es'
import { formatCurrency } from '@/shared/lib/format'
import { cn } from '@/shared/lib/utils'
import { useDeleteSubscription, useSubscriptions } from './hooks'
import { SubscriptionFormDialog } from './subscription-form-dialog'
import type { Subscription } from './types'
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

const RENEWS_SOON_THRESHOLD_DAYS = 7

function formatRenewalDate(date: string): string {
  return format(new Date(date), "d 'de' MMMM", { locale: dateFnsEs })
}

function isRenewingSoon(renewalDate: string): boolean {
  const days = differenceInCalendarDays(new Date(renewalDate), new Date())
  return days >= 0 && days <= RENEWS_SOON_THRESHOLD_DAYS
}

export default function SubscriptionsPage() {
  const { data: subscriptions, isLoading } = useSubscriptions()
  const deleteSubscription = useDeleteSubscription()

  const [formOpen, setFormOpen] = useState(false)
  const [editingSubscription, setEditingSubscription] = useState<Subscription | null>(null)
  const [subscriptionToDelete, setSubscriptionToDelete] = useState<Subscription | null>(null)

  const openCreateForm = () => {
    setEditingSubscription(null)
    setFormOpen(true)
  }

  const openEditForm = (subscription: Subscription) => {
    setEditingSubscription(subscription)
    setFormOpen(true)
  }

  const confirmDelete = () => {
    if (!subscriptionToDelete) return
    deleteSubscription.mutate(subscriptionToDelete.id, {
      onSuccess: () => setSubscriptionToDelete(null),
    })
  }

  return (
    <div className="grid gap-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:gap-4">
        <div className="min-w-0">
          <h1 className="font-heading text-2xl font-semibold">{es.subscriptions.title}</h1>
          <p className="text-muted-foreground">{es.subscriptions.subtitle}</p>
        </div>
        <Button onClick={openCreateForm} className="w-full sm:w-auto">
          <Plus className="size-4" />
          {es.subscriptions.newSubscription}
        </Button>
      </div>

      {isLoading ? (
        <div className="grid gap-2">
          {Array.from({ length: 3 }).map((_, index) => (
            <Skeleton key={index} className="h-16 w-full rounded-xl" />
          ))}
        </div>
      ) : subscriptions && subscriptions.length > 0 ? (
        <div className="grid gap-2">
          {subscriptions.map((subscription) => (
            <Card
              key={subscription.id}
              className="flex-row items-center justify-between gap-4 px-4 py-4"
            >
              <div className="flex min-w-0 items-center gap-3">
                {isRenewingSoon(subscription.renewalDate) && (
                  <Badge className="gap-1 bg-destructive/15 text-destructive">
                    {es.subscriptions.renewsSoon}
                  </Badge>
                )}
                <Badge
                  variant="outline"
                  className="gap-1"
                  title={
                    subscription.reminderEnabled
                      ? es.subscriptions.reminderOn
                      : es.subscriptions.reminderOff
                  }
                >
                  {subscription.reminderEnabled ? (
                    <Bell className="size-3" />
                  ) : (
                    <BellOff className="size-3" />
                  )}
                </Badge>
                <div className="min-w-0">
                  <p className="truncate font-medium">{subscription.name}</p>
                  <p className="font-numeric text-xs text-muted-foreground">
                    {formatRenewalDate(subscription.renewalDate)} ·{' '}
                    {formatCurrency(subscription.amount)}
                  </p>
                </div>
              </div>
              <div className="flex shrink-0 items-center gap-1">
                <Button
                  variant="ghost"
                  size="icon-sm"
                  aria-label={es.common.edit}
                  title={es.common.edit}
                  onClick={() => openEditForm(subscription)}
                >
                  <Pencil className="size-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon-sm"
                  aria-label={es.common.delete}
                  title={es.common.delete}
                  onClick={() => setSubscriptionToDelete(subscription)}
                >
                  <Trash2 className="size-4" />
                </Button>
              </div>
            </Card>
          ))}
        </div>
      ) : (
        <Card className={cn('p-10 text-center text-muted-foreground')}>
          {es.subscriptions.emptyState}
        </Card>
      )}

      <SubscriptionFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        subscription={editingSubscription}
      />

      <AlertDialog
        open={subscriptionToDelete !== null}
        onOpenChange={(open) => !open && setSubscriptionToDelete(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{es.common.confirmDeleteTitle}</AlertDialogTitle>
            <AlertDialogDescription>{es.common.confirmDeleteDescription}</AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>{es.common.cancel}</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete} disabled={deleteSubscription.isPending}>
              {deleteSubscription.isPending ? es.common.deleting : es.common.delete}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
