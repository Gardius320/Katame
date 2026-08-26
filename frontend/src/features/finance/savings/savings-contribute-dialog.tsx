import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { es } from '@/shared/i18n/es'
import { useContributeSavingsGoal } from './hooks'
import type { SavingsGoal } from './types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
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

const contributeFormSchema = z.object({
  amount: z.number().positive(es.finance.savings.validation.contributionAmountRequired),
})

type ContributeFormSchema = z.infer<typeof contributeFormSchema>

interface SavingsContributeDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  goal: SavingsGoal | null
  onContributed?: (goal: SavingsGoal) => void
}

export function SavingsContributeDialog({
  open,
  onOpenChange,
  goal,
  onContributed,
}: SavingsContributeDialogProps) {
  const contribute = useContributeSavingsGoal()

  const form = useForm<ContributeFormSchema>({
    resolver: zodResolver(contributeFormSchema),
    defaultValues: { amount: 0 },
  })

  useEffect(() => {
    if (open) {
      form.reset({ amount: 0 })
    }
  }, [open, form])

  const onSubmit = form.handleSubmit((values) => {
    if (!goal) return

    contribute.mutate(
      { id: goal.id, payload: { amount: values.amount } },
      {
        onSuccess: (updatedGoal) => {
          onOpenChange(false)
          onContributed?.(updatedGoal)
        },
      },
    )
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="font-heading">
            {es.finance.savings.addFundsTitle.replace('{name}', goal?.name ?? '')}
          </DialogTitle>
          <DialogDescription>{es.finance.savings.addFundsDescription}</DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={onSubmit} className="grid gap-4" noValidate>
            <FormField
              control={form.control}
              name="amount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>{es.finance.savings.fieldsContribute.amount}</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step="0.01"
                      className="font-numeric"
                      placeholder={es.finance.savings.fieldsContribute.amountPlaceholder}
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

            <DialogFooter className="mt-2">
              <Button type="submit" disabled={contribute.isPending} className="w-full sm:w-auto">
                {contribute.isPending ? es.common.saving : es.common.save}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
