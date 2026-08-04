import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateObligation, useUpdateObligation } from './hooks'
import type { Obligation } from './types'
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

const obligationFormSchema = z.object({
  name: z
    .string()
    .min(1, es.finance.obligations.validation.nameRequired)
    .max(100, es.finance.obligations.validation.nameMaxLength),
  amount: z.number().positive(es.finance.obligations.validation.amountRequired),
  dueDate: z.string().min(1),
  isRecurring: z.boolean(),
})

type ObligationFormSchema = z.infer<typeof obligationFormSchema>

function toDateInputValue(date: string): string {
  return date.slice(0, 10)
}

interface ObligationFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  obligation?: Obligation | null
}

export function ObligationFormDialog({
  open,
  onOpenChange,
  obligation,
}: ObligationFormDialogProps) {
  const isEditing = Boolean(obligation)
  const createObligation = useCreateObligation()
  const updateObligation = useUpdateObligation()
  const mutation = isEditing ? updateObligation : createObligation

  const form = useForm<ObligationFormSchema>({
    resolver: zodResolver(obligationFormSchema),
    defaultValues: { name: '', amount: 0, dueDate: '', isRecurring: false },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        name: obligation?.name ?? '',
        amount: obligation?.amount ?? 0,
        dueDate: obligation ? toDateInputValue(obligation.dueDate) : '',
        isRecurring: obligation?.isRecurring ?? false,
      })
    }
  }, [open, obligation, form])

  const onSubmit = form.handleSubmit((values) => {
    const dueDate = new Date(values.dueDate).toISOString()
    const onSuccess = () => onOpenChange(false)

    if (isEditing && obligation) {
      updateObligation.mutate(
        {
          id: obligation.id,
          payload: {
            name: values.name,
            amount: values.amount,
            dueDate,
            isRecurring: values.isRecurring,
            isPaid: obligation.isPaid,
          },
        },
        { onSuccess },
      )
    } else {
      createObligation.mutate(
        { name: values.name, amount: values.amount, dueDate, isRecurring: values.isRecurring },
        { onSuccess },
      )
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing
              ? es.finance.obligations.editObligation
              : es.finance.obligations.newObligation}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.obligations.fields.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.obligations.fields.namePlaceholder}
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
                  <FormLabel>{es.finance.obligations.fields.amount}</FormLabel>
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
              name="dueDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.obligations.fields.dueDate}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="isRecurring"
              render={({ field }) => (
                <FormItem className="flex flex-row items-center gap-2">
                  <FormControl>
                    <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                  </FormControl>
                  <FormLabel className="font-normal">
                    {es.finance.obligations.fields.isRecurring}
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
