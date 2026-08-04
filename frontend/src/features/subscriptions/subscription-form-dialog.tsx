import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateSubscription, useUpdateSubscription } from './hooks'
import type { Subscription } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Checkbox } from '@/shared/components/ui/checkbox'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/shared/components/ui/dialog'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/shared/components/ui/form'

const subscriptionFormSchema = z.object({
  name: z
    .string()
    .min(1, es.subscriptions.validation.nameRequired)
    .max(100, es.subscriptions.validation.nameMaxLength),
  amount: z.number().positive(es.subscriptions.validation.amountRequired),
  renewalDate: z.string().min(1),
  reminderEnabled: z.boolean(),
})

type SubscriptionFormSchema = z.infer<typeof subscriptionFormSchema>

function toDateInputValue(date: string): string {
  return date.slice(0, 10)
}

interface SubscriptionFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  subscription?: Subscription | null
}

export function SubscriptionFormDialog({
  open,
  onOpenChange,
  subscription,
}: SubscriptionFormDialogProps) {
  const isEditing = Boolean(subscription)
  const createSubscription = useCreateSubscription()
  const updateSubscription = useUpdateSubscription()
  const mutation = isEditing ? updateSubscription : createSubscription

  const form = useForm<SubscriptionFormSchema>({
    resolver: zodResolver(subscriptionFormSchema),
    defaultValues: { name: '', amount: 0, renewalDate: '', reminderEnabled: true },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        name: subscription?.name ?? '',
        amount: subscription?.amount ?? 0,
        renewalDate: subscription ? toDateInputValue(subscription.renewalDate) : '',
        reminderEnabled: subscription?.reminderEnabled ?? true,
      })
    }
  }, [open, subscription, form])

  const onSubmit = form.handleSubmit((values) => {
    const renewalDate = new Date(values.renewalDate).toISOString()
    const onSuccess = () => onOpenChange(false)
    const payload = {
      name: values.name,
      amount: values.amount,
      renewalDate,
      reminderEnabled: values.reminderEnabled,
    }

    if (isEditing && subscription) {
      updateSubscription.mutate({ id: subscription.id, payload }, { onSuccess })
    } else {
      createSubscription.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.subscriptions.editSubscription : es.subscriptions.newSubscription}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.subscriptions.fields.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.subscriptions.fields.namePlaceholder}
                      autoFocus
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="amount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.subscriptions.fields.amount}</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step="0.01"
                      className="font-numeric"
                      name={field.name}
                      onBlur={field.onBlur}
                      ref={field.ref}
                      value={field.value}
                      onChange={(e) => field.onChange(e.target.valueAsNumber)}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="renewalDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.subscriptions.fields.renewalDate}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="reminderEnabled"
              render={({ field }) => (
                <FormItem className="flex flex-row items-center gap-2">
                  <FormControl>
                    <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                  </FormControl>
                  <FormLabel className="font-normal">
                    {es.subscriptions.fields.reminderEnabled}
                  </FormLabel>
                </FormItem>
              )}
            />

            <DialogFooter className="mt-2">
              <Button type="submit" disabled={mutation.isPending} className="w-full sm:w-auto">
                {mutation.isPending ? es.common.saving : es.common.save}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
