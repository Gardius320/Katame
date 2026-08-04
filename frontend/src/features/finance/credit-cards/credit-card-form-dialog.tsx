import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateCreditCard, useUpdateCreditCard } from './hooks'
import type { CreditCard } from './types'
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

const creditCardFormSchema = z.object({
  name: z
    .string()
    .min(1, es.finance.creditCards.validation.nameRequired)
    .max(100, es.finance.creditCards.validation.nameMaxLength),
  statementDay: z
    .number()
    .int()
    .min(1, es.finance.creditCards.validation.statementDayRange)
    .max(31, es.finance.creditCards.validation.statementDayRange),
  paymentDay: z
    .number()
    .int()
    .min(1, es.finance.creditCards.validation.paymentDayRange)
    .max(31, es.finance.creditCards.validation.paymentDayRange),
  creditLimit: z.number().positive(es.finance.creditCards.validation.creditLimitRequired),
})

type CreditCardFormSchema = z.infer<typeof creditCardFormSchema>

interface CreditCardFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  card?: CreditCard | null
}

export function CreditCardFormDialog({ open, onOpenChange, card }: CreditCardFormDialogProps) {
  const isEditing = Boolean(card)
  const createCard = useCreateCreditCard()
  const updateCard = useUpdateCreditCard()
  const mutation = isEditing ? updateCard : createCard

  const form = useForm<CreditCardFormSchema>({
    resolver: zodResolver(creditCardFormSchema),
    defaultValues: { name: '', statementDay: 1, paymentDay: 1, creditLimit: 0 },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        name: card?.name ?? '',
        statementDay: card?.statementDay ?? 1,
        paymentDay: card?.paymentDay ?? 1,
        creditLimit: card?.creditLimit ?? 0,
      })
    }
  }, [open, card, form])

  const onSubmit = form.handleSubmit((values) => {
    const onSuccess = () => onOpenChange(false)

    if (isEditing && card) {
      updateCard.mutate({ id: card.id, payload: values }, { onSuccess })
    } else {
      createCard.mutate(values, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.finance.creditCards.editCard : es.finance.creditCards.newCard}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.creditCards.fields.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.creditCards.fields.namePlaceholder}
                      autoFocus
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="statementDay"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.creditCards.fields.statementDay}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        max={31}
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
                name="paymentDay"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.creditCards.fields.paymentDay}</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        max={31}
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
            </div>

            <FormField
              control={form.control}
              name="creditLimit"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.creditCards.fields.creditLimit}</FormLabel>
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
