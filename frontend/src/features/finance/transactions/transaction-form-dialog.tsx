import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateTransaction, useUpdateTransaction } from './hooks'
import { useCreditCards } from '../credit-cards/hooks'
import type { Transaction, TransactionType } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

const typeOptions: TransactionType[] = ['income', 'expense']

const NO_CARD_VALUE = 'none'

const transactionFormSchema = z.object({
  amount: z.number().positive(es.finance.transactions.validation.amountRequired),
  type: z.enum(['income', 'expense']),
  category: z
    .string()
    .min(1, es.finance.transactions.validation.categoryRequired)
    .max(50, es.finance.transactions.validation.categoryMaxLength),
  date: z.string().min(1),
  creditCardId: z.string(),
})

type TransactionFormSchema = z.infer<typeof transactionFormSchema>

function toDateInputValue(date: string): string {
  return date.slice(0, 10)
}

interface TransactionFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  transaction?: Transaction | null
}

export function TransactionFormDialog({
  open,
  onOpenChange,
  transaction,
}: TransactionFormDialogProps) {
  const isEditing = Boolean(transaction)
  const createTransaction = useCreateTransaction()
  const updateTransaction = useUpdateTransaction()
  const mutation = isEditing ? updateTransaction : createTransaction
  const { data: creditCards } = useCreditCards()

  const form = useForm<TransactionFormSchema>({
    resolver: zodResolver(transactionFormSchema),
    defaultValues: { amount: 0, type: 'expense', category: '', date: '', creditCardId: NO_CARD_VALUE },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        amount: transaction?.amount ?? 0,
        type: transaction?.type ?? 'expense',
        category: transaction?.category ?? '',
        date: transaction
          ? toDateInputValue(transaction.date)
          : toDateInputValue(new Date().toISOString()),
        creditCardId: transaction?.creditCardId ? String(transaction.creditCardId) : NO_CARD_VALUE,
      })
    }
  }, [open, transaction, form])

  const onSubmit = form.handleSubmit((values) => {
    const payload = {
      amount: values.amount,
      type: values.type,
      category: values.category,
      date: new Date(values.date).toISOString(),
      creditCardId: values.creditCardId === NO_CARD_VALUE ? null : Number(values.creditCardId),
    }

    const onSuccess = () => onOpenChange(false)

    if (isEditing && transaction) {
      updateTransaction.mutate({ id: transaction.id, payload }, { onSuccess })
    } else {
      createTransaction.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing
              ? es.finance.transactions.editTransaction
              : es.finance.transactions.newTransaction}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="amount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.transactions.fields.amount}</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step="0.01"
                      className="font-numeric"
                      autoFocus
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
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.transactions.fields.type}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {typeOptions.map((type) => (
                        <SelectItem key={type} value={type}>
                          {es.finance.transactions.type[type]}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="category"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.transactions.fields.category}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.transactions.fields.categoryPlaceholder}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="date"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.transactions.fields.date}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="creditCardId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.transactions.fields.creditCard}</FormLabel>
                  <Select value={field.value} onValueChange={field.onChange}>
                    <FormControl>
                      <SelectTrigger className="w-full">
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value={NO_CARD_VALUE}>
                        {es.finance.transactions.fields.creditCardNone}
                      </SelectItem>
                      {(creditCards ?? []).map((card) => (
                        <SelectItem key={card.id} value={String(card.id)}>
                          {card.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
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
