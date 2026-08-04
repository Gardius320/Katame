import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useCreateSavingsGoal, useUpdateSavingsGoal } from './hooks'
import type { SavingsGoal } from './types'
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

const savingsFormSchema = z.object({
  name: z
    .string()
    .min(1, es.finance.savings.validation.nameRequired)
    .max(100, es.finance.savings.validation.nameMaxLength),
  targetAmount: z.number().positive(es.finance.savings.validation.targetAmountRequired),
  currentAmount: z.number().min(0, es.finance.savings.validation.currentAmountInvalid),
  dueDate: z.string(),
})

type SavingsFormSchema = z.infer<typeof savingsFormSchema>

function toDateInputValue(date: string | null): string {
  if (!date) return ''
  return date.slice(0, 10)
}

interface SavingsFormDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  goal?: SavingsGoal | null
}

export function SavingsFormDialog({ open, onOpenChange, goal }: SavingsFormDialogProps) {
  const isEditing = Boolean(goal)
  const createGoal = useCreateSavingsGoal()
  const updateGoal = useUpdateSavingsGoal()
  const mutation = isEditing ? updateGoal : createGoal

  const form = useForm<SavingsFormSchema>({
    resolver: zodResolver(savingsFormSchema),
    defaultValues: { name: '', targetAmount: 0, currentAmount: 0, dueDate: '' },
  })

  useEffect(() => {
    if (open) {
      form.reset({
        name: goal?.name ?? '',
        targetAmount: goal?.targetAmount ?? 0,
        currentAmount: goal?.currentAmount ?? 0,
        dueDate: toDateInputValue(goal?.dueDate ?? null),
      })
    }
  }, [open, goal, form])

  const onSubmit = form.handleSubmit((values) => {
    const payload = {
      name: values.name,
      targetAmount: values.targetAmount,
      currentAmount: values.currentAmount,
      dueDate: values.dueDate ? new Date(values.dueDate).toISOString() : null,
    }

    const onSuccess = () => onOpenChange(false)

    if (isEditing && goal) {
      updateGoal.mutate({ id: goal.id, payload }, { onSuccess })
    } else {
      createGoal.mutate(payload, { onSuccess })
    }
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {isEditing ? es.finance.savings.editGoal : es.finance.savings.newGoal}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.savings.fields.name}</FormLabel>
                  <FormControl>
                    <Input
                      placeholder={es.finance.savings.fields.namePlaceholder}
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
              name="targetAmount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.savings.fields.targetAmount}</FormLabel>
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
              name="currentAmount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.savings.fields.currentAmount}</FormLabel>
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
                  <FormLabel>{es.finance.savings.fields.dueDate}</FormLabel>
                  <FormControl>
                    <Input type="date" className="font-numeric" {...field} />
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
