import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateObligation, useUpdateObligation } from './hooks'
import type { Obligation, RecurrenceFrequency } from './types'
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select'

const recurrenceFrequencyOptions: RecurrenceFrequency[] = ['Biweekly', 'Monthly']

const obligationFormSchema = z
  .object({
    name: z
      .string()
      .min(1, es.finance.obligations.validation.nameRequired)
      .max(100, es.finance.obligations.validation.nameMaxLength),
    amount: z.number().positive(es.finance.obligations.validation.amountRequired),
    dueDate: z.string(),
    isRecurring: z.boolean(),
    recurrenceFrequency: z.enum(['Biweekly', 'Monthly']).nullable(),
  })
  .refine((data) => data.isRecurring || data.dueDate.length > 0, {
    message: es.finance.obligations.validation.dueDateRequired,
    path: ['dueDate'],
  })
  .refine((data) => !data.isRecurring || data.recurrenceFrequency !== null, {
    message: es.finance.obligations.validation.recurrenceFrequencyRequired,
    path: ['recurrenceFrequency'],
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
    defaultValues: {
      name: '',
      amount: 0,
      dueDate: '',
      isRecurring: false,
      recurrenceFrequency: null,
    },
  })

  const isRecurring = form.watch('isRecurring')

  useEffect(() => {
    if (open) {
      const recurring = obligation?.isRecurring ?? false
      form.reset({
        name: obligation?.name ?? '',
        amount: obligation?.amount ?? 0,
        dueDate: obligation && !recurring ? toDateInputValue(obligation.dueDate) : '',
        isRecurring: recurring,
        // Si ya es recurrente pero no tiene frecuencia guardada (dato viejo, de
        // antes de que existiera este campo), se asume mensual por defecto.
        recurrenceFrequency: recurring ? (obligation?.recurrenceFrequency ?? 'Monthly') : null,
      })
    }
  }, [open, obligation, form])

  const onSubmit = form.handleSubmit((values) => {
    // Las obligaciones recurrentes no piden fecha de calendario: solo quincenal o
    // mensual. Igual mandamos una fecha porque el backend la necesita, pero no la
    // pedimos ni la mostramos en el formulario para este caso.
    const dueDate = values.isRecurring
      ? (obligation?.dueDate ?? new Date().toISOString())
      : new Date(values.dueDate).toISOString()
    const recurrenceFrequency = values.isRecurring ? values.recurrenceFrequency : null
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
            recurrenceFrequency,
            isPaid: obligation.isPaid,
          },
        },
        { onSuccess },
      )
    } else {
      createObligation.mutate(
        {
          name: values.name,
          amount: values.amount,
          dueDate,
          isRecurring: values.isRecurring,
          recurrenceFrequency,
        },
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
              name="isRecurring"
              render={({ field }) => (
                <FormItem className="flex flex-row items-center gap-2">
                  <FormControl>
                    <Checkbox
                      checked={field.value}
                      onCheckedChange={(checked) => {
                        field.onChange(checked)
                        if (checked) {
                          if (!form.getValues('recurrenceFrequency')) {
                            form.setValue('recurrenceFrequency', 'Monthly')
                          }
                        } else {
                          form.setValue('recurrenceFrequency', null)
                        }
                      }}
                    />
                  </FormControl>
                  <FormLabel className="font-normal">
                    {es.finance.obligations.fields.isRecurring}
                  </FormLabel>
                </FormItem>
              )}
            />

            {isRecurring ? (
              <FormField
                control={form.control}
                name="recurrenceFrequency"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>{es.finance.obligations.fields.recurrenceFrequency}</FormLabel>
                    <Select value={field.value ?? 'Monthly'} onValueChange={field.onChange}>
                      <FormControl>
                        <SelectTrigger className="w-full">
                          <SelectValue />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {recurrenceFrequencyOptions.map((frequency) => (
                          <SelectItem key={frequency} value={frequency}>
                            {frequency === 'Biweekly'
                              ? es.finance.obligations.recurrenceFrequency.biweekly
                              : es.finance.obligations.recurrenceFrequency.monthly}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : (
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
            )}

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
